/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          GlobalWebSockets.h
*  Purpose :       GlobalWebSockets class.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#pragma once

#include "stdafx.h"

#ifdef NEQUEONETWEBSOCKETS_EXPORTS
#define EXPORT_NEQUEO_WEBSOCKETS_QT_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_WEBSOCKETS_QT_API __declspec(dllimport) 
#endif

#include <QtCore\qobject.h>
#include <QtCore\qconfig.h>
#include <QtCore\qglobal.h>
#include <QtCore\qalgorithms.h>
#include <QtCore\qobjectdefs.h>
#include <QtCore\QList>
#include <QtCore\QString>
#include <QtCore\QUrl>
#include <QtCore\QFile>
#include <QtCore\qmetatype.h>

#include <QtNetwork\QSslError>
#include <QtNetwork\qabstractsocket.h>
#include <QtNetwork\qauthenticator.h>
#include <QtNetwork\QSslCertificate>
#include <QtNetwork\QSslKey>

#include <QtWebSockets\QWebSocket>
#include <QtWebSockets\QWebSocketServer>