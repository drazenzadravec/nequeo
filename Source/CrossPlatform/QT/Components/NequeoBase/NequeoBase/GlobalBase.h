#pragma once

#include "stdafx.h"

#ifdef NEQUEOBASE_EXPORTS
#define EXPORT_NEQUEO_BASE_QT_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_BASE_QT_API __declspec(dllimport) 
#endif

#include <QtCore\qconfig.h>
#include <QtCore\qglobal.h>
#include <QtCore\qalgorithms.h>