
// MFCApplicationTest.h : main header file for the MFCApplicationTest application
//
#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols


// CMFCApplicationTestApp:
// See MFCApplicationTest.cpp for the implementation of this class
//

class CMFCApplicationTestApp : public CWinApp
{
public:
	CMFCApplicationTestApp();


// Overrides
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

// Implementation

public:
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern CMFCApplicationTestApp theApp;
