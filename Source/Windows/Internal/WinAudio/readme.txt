WinAudio: Windows Audio Sample (Win32, C++)

Copyright (c) Microsoft Corporation. All Rights Reserved



What does the sample do?
=========================

	This Windows audio ("WinAudio") sample is a Win32-based application that demonstrates the use of the core audio APIs in Windows Vista. The sample uses the Multimedia Device (MMDevice) API and the Windows Audio Service API (WASAPI). WinAudio presents a dialog window that contains controls for selecting audio devices and playing audio streams. The sample is written in C++.

	WinAudio does *not* run on earlier versions of Windows, including Windows XP, Windows 2000, Windows Me, and Windows 98.


How to build the sample
=========================

	Install the Platform SDK. To build the WinAudio sample application from the command line, open the CMD shell for the Windows SDK from the Windows Start button as follows:

	Start -> All Programs -> Microsoft Windows SDK -> CMD Shell

Note that running the CMD shell for the SDK automatically sets the environment variables that are required to build the sample. If you choose to open a different CMD shell that does not set up the SDK build environment, you will have to set the path, INCLUDE, and LIB environment variables manually.

	From within the CMD shell, execute the following command to change to the WinAudio sample directory:

	cd /d %MSSdk%\samples\Multimedia\Audio\WinAudio

MSSdk is an environment variable that specifies the path to the main SDK directory. From within the WinAudio directory, execute the following two commands to build the debug and release versions of the WinAudio executable file, WinAudio.exe:

	vcbuild WinAudio.vcproj Debug
	vcbuild WinAudio.vcproj Release

The resulting debug and release versions of WinAudio.exe will be located in the Debug and Release subdirectories, respectively.

You can also build the WinAudio sample application from within Visual Studio 2005. Open the CMD shell for the Windows SDK and change to the WinAudio sample directory, as explained previously. Run the command "start WinAudio.sln" in the WinAudio directory to open the WinAudio project in the Visual Studio window. From within the window, select the Debug or Release solution configuration, select the Build menu from the menu bar, and select the Build option. Note that if you do not open Visual Studio from the CMD shell for the SDK, Visual Studio will not have access to the SDK build environment. In that case, the sample will not build unless you explicitly set environment variable MSSdk, which is used in the project file, WinAudio.vcproj.


How to run the sample
=========================

	Run the WinAudio executable file, WinAudio.exe, in Windows Vista. The WinAudio application presents a dialog window with controls for selecting endpoint playback and capture devices, opening wave files, and controlling the volume level.

	To play a continuous tone through an endpoint rendering device (for example, speakers or headphones), select an endpoint rendering device in the "Playback Device" box in the dialog window. Next, select the "Tone Generator" in the "Audio Source" box. Finally, click on the "Play" button in the "Player Controls" box. If the sound is too loud, adjust the volume slider in the "Playback Volume" box accordingly. Click on the "Stop" button to stop the sound.

	To play the stream from an endpoint capture device (for example, a microphone) through the currently selected endpoint rendering device, select a capture device in the "Capture Device" box. Next, click on the "Play" button to play the capture stream through the currently selected endpoint rendering device.

	To play a wave file through the currently selected endpoint rendering device, click on the "Select File" button in the "Wave File" box, choose a wave file (with a .wav file name extension), and open it. When you click on the "Play" button, the rendering device will play the file only if it is encoded in a wave format that the audio engine supports. In Windows Vista, the audio engine is the system software mixer. Its role is similar to that of the kernel mixer, KMixer, in earlier versions of Windows.

	The sample application can play only wave files that have wave formats that are compatible with the wave format of the audio engine's mix. If you attempt to play an incompatible wave file, the sample application will pop up a message box explaining that it cannot play the file.

	Typically, the engine cannot play a wave file with a sample rate that does not match the sample rate of the mix. In addition, the engine can only play integer and floating-point PCM data. By default, the engine cannot play other audio data formats, such as ADPCM and WMA, although third parties might provide plug-in system effects that allow the engine to accept a wider range of audio formats.

	Note that this sample does *not* provide a means to record an audio stream from an endpoint capture device (for example, microphone) to a wave file. If you select a capture device as the audio source, the sample simply plays the stream from the capture device through the selected playback device.


Targeted platforms
=========================

	WinAudio runs on 32-bit and 64-bit Windows Vista platforms.
	

APIs used in the sample
=========================

	 Windows Vista supports the following core audio APIs:
- MMDevice API (multimedia device enumeration and selection)
- WASAPI (Windows audio services for managing audio streams)
- Topology API (discovery of the control topologies of audio hardware devices)

The current implementation of WinAudio demonstrates the use of the MMDevice API and WASAPI, but not the Topology API.


Known limitations
=========================

	The current implementation of WinAudio plays audio streams only in shared mode. Future versions of WinAudio might be able to play streams in either or exclusive mode or shared mode.


File manifest
=========================

source.h        | Defines the AudioSource interface, which encapsulates an audio source.
capture.cpp     | Implements an audio thread to play a stream from a capture device.
large.ico       | A large icon for the WinAudio application.
player.cpp      | Implements the methods in the Player class, which plays audio streams.
player.h        | Defines the Player class.
playwave.cpp    | Implements an audio thread to play a wave file or test sine wave.
resource.h      | Defines control IDs and other constants for the graphical user interface.
sinewave.cpp    | Implements a tone generator (simple sine wave).
small.ico       | A small icon for the WinAudio application.
WinAudio.cpp    | Implements the graphical user interface for WinAudio.
WinAudio.rc     | Resource script that defines the controls in the graphical user interface.
validwfx.cpp    | Function that determines whether a wave format descriptor is valid.
wavefile.cpp    | Implements methods for parsing and wave file and reading its wave data.
WinAudio.vcproj | The Visual C/C++ project file.
WinAudio.sln    | The Visual C/C++ solution file.
readme.txt      | This readme file.

