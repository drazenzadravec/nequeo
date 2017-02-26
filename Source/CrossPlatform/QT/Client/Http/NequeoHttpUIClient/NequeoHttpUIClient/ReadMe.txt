========================================================================
    CONSOLE APPLICATION : NequeoHttpUIClient Project Overview
========================================================================

AppWizard has created this NequeoHttpUIClient application for you.

This file contains a summary of what you will find in each of the files that
make up your NequeoHttpUIClient application.


NequeoHttpUIClient.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

NequeoHttpUIClient.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).

NequeoHttpUIClient.cpp
    This is the main application source file.

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named NequeoHttpUIClient.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////

Create ui headers from ui
uic.exe -o ui_authenticationdialog.h authenticationdialog.ui

Create moc files from headers
moc.exe -o moc_httpwindow.cpp httpwindow.h