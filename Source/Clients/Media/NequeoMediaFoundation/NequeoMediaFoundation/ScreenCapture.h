/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ScreenCapture.h
*  Purpose :       ScreenCapture class.
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

#ifndef _SCREENCAPTURE_H
#define _SCREENCAPTURE_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation screen capture.
			/// </summary>
			class ScreenCapture
			{
			public:

				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="hwnd">The handle to the application owner.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API ScreenCapture(HWND hwnd);

				/// <summary>
				/// This destructor. Call release to cleanup resources.
				/// </summary>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API virtual ~ScreenCapture();

				/// <summary>
				/// Capture the screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToFile(const WCHAR *pwszFileName);

				/// <summary>
				/// Capture the primary screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToFileSize(const WCHAR *pwszFileName, int nWidth, int nHeight);

				/// <summary>
				/// Capture the primary screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToFile(const WCHAR *pwszFileName, int nXSrc, int nYSrc);

				/// <summary>
				/// Capture the primary screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToFile(const WCHAR *pwszFileName, int nWidth, int nHeight, int nXSrc, int nYSrc);

				/// <summary>
				/// Capture the primary screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToFile(const WCHAR *pwszFileName, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc);

				/// <summary>
				/// Capture the screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToBitmap(BITMAP *bmpScreen);

				/// <summary>
				/// Capture the primary screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToBitmapSize(BITMAP *bmpScreen, int nWidth, int nHeight);

				/// <summary>
				/// Capture the primary screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToBitmap(BITMAP *bmpScreen, int nXSrc, int nYSrc);

				/// <summary>
				/// Capture the primary screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToBitmap(BITMAP *bmpScreen, int nWidth, int nHeight, int nXSrc, int nYSrc);

				/// <summary>
				/// Capture the primary screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int ImageToBitmap(BITMAP *bmpScreen, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc);

				/// <summary>
				/// The coordinates for the left side of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
				/// The SM_CXVIRTUALSCREEN metric is the width of the virtual screen. 
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_XVIRTUALSCREEN();

				/// <summary>
				/// The coordinates for the top of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
				/// The SM_CYVIRTUALSCREEN metric is the height of the virtual screen. 
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_YVIRTUALSCREEN();

				/// <summary>
				/// The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
				/// The SM_XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen. 
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CXVIRTUALSCREEN();

				/// <summary>
				/// The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
				/// The SM_YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CYVIRTUALSCREEN();

				/// <summary>
				/// The width of the client area for a full-screen window on the primary display monitor, in pixels. 
				/// To get the coordinates of the portion of the screen that is not obscured by the system taskbar or 
				/// by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CXFULLSCREEN();

				/// <summary>
				/// The height of the client area for a full-screen window on the primary display monitor, in pixels. 
				/// To get the coordinates of the portion of the screen not obscured by the system taskbar or by application 
				/// desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CYFULLSCREEN();

				/// <summary>
				/// The width of the screen of the primary display monitor, in pixels. This is the same value obtained by 
				/// calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CXSCREEN();

				/// <summary>
				/// The height of the screen of the primary display monitor, in pixels. This is the same value obtained 
				/// by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
				/// </summary>
				/// <return>The value.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API int Get_SM_CYSCREEN();

				/// <summary>
				/// Set the scalling size.
				/// </summary>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="hwndScalling">The scalling capture handler.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API void Scale(int nWidth, int nHeight, HWND hwndScalling = NULL);

				/// <summary>
				/// Get the scalling indicator.
				/// </summary>
				/// <return>True if scalling is on; else false.</return>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API bool GetIsScalling();

				/// <summary>
				/// Set the scalling indicator.
				/// </summary>
				/// <param name="scalling">True if scalling is on; else false.</param>
				EXPORT_NEQUEO_MEDIA_FOUNDATION_API void SetIsScalling(bool scalling);

			private:
				bool _disposed;
				bool _scalling;
				int _scallingWidth;
				int _scallingHeight;

				HWND _hwnd;
				HWND _hwndScalling;

				/// <summary>
				/// Capture the screen to a file.
				/// </summary>
				/// <param name="pwszFileName">The path and name of the file.</param>
				/// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				int ImageToFileEx(const WCHAR *pwszFileName, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int index);

				/// <summary>
				/// Capture the screen to a bitmap.
				/// </summary>
				/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
				/// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
				/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
				/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
				/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
				/// <return>Zero if no error; else non zero.</return>
				int ImageToBitmapEx(BITMAP *bmpScreen, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int index);
			};
		}
	}
}
#endif