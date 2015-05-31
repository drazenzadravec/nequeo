//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    Provides common includes and definitions used by the 
//                  project and reduces build time.
//
//*****************************************************************************

#pragma once

#define _WIN32_WINNT 0x0501

#include <windows.h>
#include <wincred.h>
#include <vcclr.h>
#include <msclr\auto_handle.h>

using namespace System;
using namespace Security::Permissions;

using Diagnostics::Debug;


