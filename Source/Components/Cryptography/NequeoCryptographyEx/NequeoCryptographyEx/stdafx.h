// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // some CString constructors will be explicit
#define _AFX_NO_MFC_CONTROLS_IN_DIALOGS         // remove support for MFC controls in dialogs

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // Exclude rarely-used stuff from Windows headers
#endif

// MFC core and standard components
#include <afx.h>
#include <afxwin.h>         
#include <afxdlgs.h>
#include <afxcmn.h>

// C RunTime Header Files
#include <tchar.h>
#include <commdlg.h> // OpenFile dialog

// Safe string functions
#include <strsafe.h>

// Include the standard cryptography header.
#include <Wincrypt.h>