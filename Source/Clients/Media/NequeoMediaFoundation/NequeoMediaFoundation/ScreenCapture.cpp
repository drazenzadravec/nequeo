/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ScreenCapture.cpp
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

#include "stdafx.h"

#include "ScreenCapture.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			/// <param name="hwnd">The handle to the application owner.</param>
			ScreenCapture::ScreenCapture(HWND hwnd) :
				_hwnd(hwnd),
				_hwndScalling(NULL),
				_scalling(false),
				_scallingWidth(0),
				_scallingHeight(0),
				_disposed(false)
			{

			}

			/// <summary>
			/// This destructor. Call release to cleanup resources.
			/// </summary>
			ScreenCapture::~ScreenCapture()
			{
				if (!_disposed)
				{
					_disposed = true;
				}
			}

			/// <summary>
			/// Capture the screen to a file.
			/// </summary>
			/// <param name="pwszFileName">The path and name of the file.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToFile(const WCHAR *pwszFileName)
			{
				return ImageToFileEx(pwszFileName, 0, 0, 0, 0, 0, 0, 0);
			}

			/// <summary>
			/// Capture the primary screen to a file.
			/// </summary>
			/// <param name="pwszFileName">The path and name of the file.</param>
			/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
			/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToFileSize(const WCHAR *pwszFileName, int nWidth, int nHeight)
			{
				return ImageToFileEx(pwszFileName, 0, 0, nWidth, nHeight, 0, 0, 4);
			}

			/// <summary>
			/// Capture the primary screen to a file.
			/// </summary>
			/// <param name="pwszFileName">The path and name of the file.</param>
			/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToFile(const WCHAR *pwszFileName, int nXSrc, int nYSrc)
			{
				return ImageToFileEx(pwszFileName, 0, 0, 0, 0, nXSrc, nYSrc, 3);
			}

			/// <summary>
			/// Capture the primary screen to a file.
			/// </summary>
			/// <param name="pwszFileName">The path and name of the file.</param>
			/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
			/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
			/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToFile(const WCHAR *pwszFileName, int nWidth, int nHeight, int nXSrc, int nYSrc)
			{
				return ImageToFileEx(pwszFileName, 0, 0, nWidth, nHeight, nXSrc, nYSrc, 2);
			}

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
			int ScreenCapture::ImageToFile(const WCHAR *pwszFileName, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc)
			{
				return ImageToFileEx(pwszFileName, nXDest, nYDest, nWidth, nHeight, nXSrc, nYSrc, 1);
			}

			/// <summary>
			/// Capture the screen to a bitmap.
			/// </summary>
			/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToBitmap(BITMAP *bmpScreen)
			{
				return ImageToBitmapEx(bmpScreen, 0, 0, 0, 0, 0, 0, 0);
			}

			/// <summary>
			/// Capture the primary screen to a bitmap.
			/// </summary>
			/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
			/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
			/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToBitmapSize(BITMAP *bmpScreen, int nWidth, int nHeight)
			{
				return ImageToBitmapEx(bmpScreen, 0, 0, nWidth, nHeight, 0, 0, 4);
			}

			/// <summary>
			/// Capture the primary screen to a bitmap.
			/// </summary>
			/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
			/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToBitmap(BITMAP *bmpScreen, int nXSrc, int nYSrc)
			{
				return ImageToBitmapEx(bmpScreen, 0, 0, 0, 0, nXSrc, nYSrc, 3);
			}

			/// <summary>
			/// Capture the primary screen to a bitmap.
			/// </summary>
			/// <param name="bmpScreen">The screen captured bitmap (data is stored in bmBits as char*).</param>
			/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
			/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
			/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
			/// <return>Zero if no error; else non zero.</return>
			int ScreenCapture::ImageToBitmap(BITMAP *bmpScreen, int nWidth, int nHeight, int nXSrc, int nYSrc)
			{
				return ImageToBitmapEx(bmpScreen, 0, 0, nWidth, nHeight, nXSrc, nYSrc, 2);
			}

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
			int ScreenCapture::ImageToBitmap(BITMAP *bmpScreen, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc)
			{
				return ImageToBitmapEx(bmpScreen, nXDest, nYDest, nWidth, nHeight, nXSrc, nYSrc, 1);
			}

			/// <summary>
			/// The coordinates for the left side of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
			/// The SM_CXVIRTUALSCREEN metric is the width of the virtual screen. 
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_XVIRTUALSCREEN()
			{
				return GetSystemMetrics(SM_XVIRTUALSCREEN);
			}

			/// <summary>
			/// The coordinates for the top of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
			/// The SM_CYVIRTUALSCREEN metric is the height of the virtual screen. 
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_YVIRTUALSCREEN()
			{
				return GetSystemMetrics(SM_YVIRTUALSCREEN);
			}

			/// <summary>
			/// The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
			/// The SM_XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen. 
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CXVIRTUALSCREEN()
			{
				return GetSystemMetrics(SM_CXVIRTUALSCREEN);
			}

			/// <summary>
			/// The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
			/// The SM_YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CYVIRTUALSCREEN()
			{
				return GetSystemMetrics(SM_CYVIRTUALSCREEN);
			}

			/// <summary>
			/// The width of the client area for a full-screen window on the primary display monitor, in pixels. 
			/// To get the coordinates of the portion of the screen that is not obscured by the system taskbar or 
			/// by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CXFULLSCREEN()
			{
				return GetSystemMetrics(SM_CXFULLSCREEN);;
			}

			/// <summary>
			/// The height of the client area for a full-screen window on the primary display monitor, in pixels. 
			/// To get the coordinates of the portion of the screen not obscured by the system taskbar or by application 
			/// desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CYFULLSCREEN()
			{
				return GetSystemMetrics(SM_CYFULLSCREEN);
			}

			/// <summary>
			/// The width of the screen of the primary display monitor, in pixels. This is the same value obtained by 
			/// calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CXSCREEN()
			{
				return GetSystemMetrics(SM_CXSCREEN);
			}

			/// <summary>
			/// The height of the screen of the primary display monitor, in pixels. This is the same value obtained 
			/// by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
			/// </summary>
			/// <return>The value.</return>
			int ScreenCapture::Get_SM_CYSCREEN()
			{
				return GetSystemMetrics(SM_CYSCREEN);
			}

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
			int ScreenCapture::ImageToFileEx(const WCHAR *pwszFileName, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int index)
			{
				HDC hdcScreen;
				HDC hdcMemDC = NULL;
				HDC hdcWindow = NULL;
				HBITMAP hbmScreen = NULL;
				BITMAP bmpScreen;

				// Retrieve the handle to a display device context for the client 
				// area of the window. 
				hdcScreen = GetDC(NULL);
				hdcWindow = GetDC(_hwndScalling);

				int x1, y1, x2, y2, w, h, dx, dy;

				// Get screen dimensions
				x1 = GetSystemMetrics(SM_XVIRTUALSCREEN);
				y1 = GetSystemMetrics(SM_YVIRTUALSCREEN);
				x2 = GetSystemMetrics(SM_CXVIRTUALSCREEN);
				y2 = GetSystemMetrics(SM_CYVIRTUALSCREEN);
				w = x2;
				h = y2 - y1;
				dx = 0;
				dy = 0;

				// Get primary.
				if (index == 1)
				{
					x1 = nXSrc;
					y1 = nYSrc;
					w = nWidth;
					h = nHeight;
					dx = nXDest;
					dy = nYDest;
				}

				// Get primary.
				if (index == 2)
				{
					x1 = nXSrc;
					y1 = nYSrc;
					w = nWidth;
					h = nHeight;
				}

				// Get primary.
				if (index == 3)
				{
					x1 = nXSrc;
					y1 = nYSrc;
				}

				// Get primary.
				if (index == 4)
				{
					w = nWidth;
					h = nHeight;
				}

				// If scaling
				if (_scalling)
				{
					// Create a compatible DC which is used in a BitBlt from the window DC
					hdcMemDC = CreateCompatibleDC(hdcWindow);

					if (!hdcMemDC)
					{
						MessageBox(_hwnd, L"CreateCompatibleDC has failed", L"Failed", MB_OK);
						goto done;
					}

					RECT rcClient;
					GetClientRect(_hwndScalling, &rcClient);

					//This is the best stretch mode
					SetStretchBltMode(hdcWindow, HALFTONE);

					// The source DC is the entire screen and the destination DC is constructed.
					if (!StretchBlt(
						hdcWindow,
						dx, dy,
						_scallingWidth, _scallingHeight,
						hdcScreen,
						x1, y1,
						w, h,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"StretchBlt has failed", L"Failed", MB_OK);
						goto done;
					}

					w = _scallingWidth;
					h = _scallingHeight;
					x1 = 0;
					y1 = 0;
					dx = 0;
					dy = 0;

					// Create a compatible bitmap from the Window DC
					hbmScreen = CreateCompatibleBitmap(hdcWindow, w, h);

					if (!hbmScreen)
					{
						MessageBox(_hwnd, L"CreateCompatibleBitmap Failed", L"Failed", MB_OK);
						goto done;
					}

					// Select the compatible bitmap into the compatible memory DC.
					SelectObject(hdcMemDC, hbmScreen);

					// Bit block transfer into our compatible memory DC.
					if (!BitBlt(
						hdcMemDC,
						dx, dy,
						w, h,
						hdcWindow,
						x1, y1,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"BitBlt has failed", L"Failed", MB_OK);
						goto done;
					}
				}
				else
				{
					// Create a compatible DC which is used in a BitBlt from the window DC
					hdcMemDC = CreateCompatibleDC(hdcScreen);

					if (!hdcMemDC)
					{
						MessageBox(_hwnd, L"CreateCompatibleDC has failed", L"Failed", MB_OK);
						goto done;
					}

					// Create a compatible bitmap from the Window DC
					hbmScreen = CreateCompatibleBitmap(hdcScreen, w, h);

					if (!hbmScreen)
					{
						MessageBox(_hwnd, L"CreateCompatibleBitmap Failed", L"Failed", MB_OK);
						goto done;
					}

					// Select the compatible bitmap into the compatible memory DC.
					SelectObject(hdcMemDC, hbmScreen);

					// Bit block transfer into our compatible memory DC.
					if (!BitBlt(
						hdcMemDC,
						dx, dy,
						w, h,
						hdcScreen,
						x1, y1,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"BitBlt has failed", L"Failed", MB_OK);
						goto done;
					}
				}

				// Get the BITMAP from the HBITMAP
				GetObject(hbmScreen, sizeof(BITMAP), &bmpScreen);

				BITMAPFILEHEADER   bmfHeader;
				BITMAPINFOHEADER   bi;

				bi.biSize = sizeof(BITMAPINFOHEADER);
				bi.biWidth = bmpScreen.bmWidth;
				bi.biHeight = bmpScreen.bmHeight;
				bi.biPlanes = 1;
				bi.biBitCount = 32;
				bi.biCompression = BI_RGB;
				bi.biSizeImage = 0;
				bi.biXPelsPerMeter = 0;
				bi.biYPelsPerMeter = 0;
				bi.biClrUsed = 0;
				bi.biClrImportant = 0;

				DWORD dwBmpSize = ((bmpScreen.bmWidth * bi.biBitCount + 31) / 32) * 4 * bmpScreen.bmHeight;

				// Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that 
				// call HeapAlloc using a handle to the process's default heap. Therefore, GlobalAlloc and LocalAlloc 
				// have greater overhead than HeapAlloc.
				HANDLE hDIB = GlobalAlloc(GHND, dwBmpSize);
				char *lpbitmap = (char *)GlobalLock(hDIB);

				// If scaling
				if (_scalling)
				{
					// Gets the "bits" from the bitmap and copies them into a buffer 
					// which is pointed to by lpbitmap.
					GetDIBits(hdcWindow, hbmScreen, 0,
						(UINT)bmpScreen.bmHeight,
						lpbitmap,
						(BITMAPINFO *)&bi, DIB_RGB_COLORS);
				}
				else
				{
					// Gets the "bits" from the bitmap and copies them into a buffer 
					// which is pointed to by lpbitmap.
					GetDIBits(hdcScreen, hbmScreen, 0,
						(UINT)bmpScreen.bmHeight,
						lpbitmap,
						(BITMAPINFO *)&bi, DIB_RGB_COLORS);
				}

				// Gets the "bits" from the bitmap and copies them into a buffer 
				// which is pointed to by lpbitmap.
				GetDIBits(hdcScreen, hbmScreen, 0,
					(UINT)bmpScreen.bmHeight,
					lpbitmap,
					(BITMAPINFO *)&bi, DIB_RGB_COLORS);

				// A file is created, this is where we will save the screen capture.
				HANDLE hFile = CreateFile(
					pwszFileName,
					GENERIC_WRITE,
					0,
					NULL,
					CREATE_ALWAYS,
					FILE_ATTRIBUTE_NORMAL, NULL);

				// Add the size of the headers to the size of the bitmap to get the total file size
				DWORD dwSizeofDIB = dwBmpSize + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

				//Offset to where the actual bitmap bits start.
				bmfHeader.bfOffBits = (DWORD)sizeof(BITMAPFILEHEADER) + (DWORD)sizeof(BITMAPINFOHEADER);

				//Size of the file
				bmfHeader.bfSize = dwSizeofDIB;

				//bfType must always be BM for Bitmaps
				bmfHeader.bfType = 0x4D42; //BM   

				DWORD dwBytesWritten = 0;
				WriteFile(hFile, (LPSTR)&bmfHeader, sizeof(BITMAPFILEHEADER), &dwBytesWritten, NULL);
				WriteFile(hFile, (LPSTR)&bi, sizeof(BITMAPINFOHEADER), &dwBytesWritten, NULL);
				WriteFile(hFile, (LPSTR)lpbitmap, dwBmpSize, &dwBytesWritten, NULL);

				//Unlock and Free the DIB from the heap
				GlobalUnlock(hDIB);
				GlobalFree(hDIB);

				//Close the handle for the file that was created
				CloseHandle(hFile);

				//Clean up
			done:
				DeleteObject(hbmScreen);
				DeleteObject(hdcMemDC);
				ReleaseDC(NULL, hdcWindow);
				ReleaseDC(NULL, hdcScreen);

				return 0;
			}

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
			int ScreenCapture::ImageToBitmapEx(BITMAP *bmpScreen, int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int index)
			{
				HDC hdcScreen;
				HDC hdcMemDC = NULL;
				HDC hdcWindow = NULL;
				HBITMAP hbmScreen = NULL;

				// Retrieve the handle to a display device context for the client 
				// area of the window. 
				hdcScreen = GetDC(NULL);
				hdcWindow = GetDC(_hwndScalling);

				int x1, y1, x2, y2, w, h, dx, dy;

				// Get screen dimensions
				x1 = GetSystemMetrics(SM_XVIRTUALSCREEN);
				y1 = GetSystemMetrics(SM_YVIRTUALSCREEN);
				x2 = GetSystemMetrics(SM_CXVIRTUALSCREEN);
				y2 = GetSystemMetrics(SM_CYVIRTUALSCREEN);
				w = x2;
				h = y2 - y1;
				dx = 0;
				dy = 0;

				// Get primary.
				if (index == 1)
				{
					x1 = nXSrc;
					y1 = nYSrc;
					w = nWidth;
					h = nHeight;
					dx = nXDest;
					dy = nYDest;
				}

				// Get primary.
				if (index == 2)
				{
					x1 = nXSrc;
					y1 = nYSrc;
					w = nWidth;
					h = nHeight;
				}

				// Get primary.
				if (index == 3)
				{
					x1 = nXSrc;
					y1 = nYSrc;
				}

				// Get primary.
				if (index == 4)
				{
					w = nWidth;
					h = nHeight;
				}

				// If scaling
				if (_scalling)
				{
					// Create a compatible DC which is used in a BitBlt from the window DC
					hdcMemDC = CreateCompatibleDC(hdcWindow);

					if (!hdcMemDC)
					{
						MessageBox(_hwnd, L"CreateCompatibleDC has failed", L"Failed", MB_OK);
						goto done;
					}

					//This is the best stretch mode
					SetStretchBltMode(hdcWindow, HALFTONE);

					// The source DC is the entire screen and the destination DC is constructed.
					if (!StretchBlt(
						hdcWindow,
						dx, dy,
						_scallingWidth, _scallingHeight,
						hdcScreen,
						x1, y1,
						w, h,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"StretchBlt has failed", L"Failed", MB_OK);
						goto done;
					}

					w = _scallingWidth;
					h = _scallingHeight;
					x1 = 0;
					y1 = 0;
					dx = 0;
					dy = 0;

					// Create a compatible bitmap from the Window DC
					hbmScreen = CreateCompatibleBitmap(hdcWindow, w, h);

					if (!hbmScreen)
					{
						MessageBox(_hwnd, L"CreateCompatibleBitmap Failed", L"Failed", MB_OK);
						goto done;
					}

					// Select the compatible bitmap into the compatible memory DC.
					SelectObject(hdcMemDC, hbmScreen);

					// Bit block transfer into our compatible memory DC.
					if (!BitBlt(
						hdcMemDC,
						dx, dy,
						w, h,
						hdcWindow,
						x1, y1,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"BitBlt has failed", L"Failed", MB_OK);
						goto done;
					}
				}
				else
				{
					// Create a compatible DC which is used in a BitBlt from the window DC
					hdcMemDC = CreateCompatibleDC(hdcScreen);

					if (!hdcMemDC)
					{
						MessageBox(_hwnd, L"CreateCompatibleDC has failed", L"Failed", MB_OK);
						goto done;
					}

					// Create a compatible bitmap from the Window DC
					hbmScreen = CreateCompatibleBitmap(hdcScreen, w, h);

					if (!hbmScreen)
					{
						MessageBox(_hwnd, L"CreateCompatibleBitmap Failed", L"Failed", MB_OK);
						goto done;
					}

					// Select the compatible bitmap into the compatible memory DC.
					SelectObject(hdcMemDC, hbmScreen);

					// Bit block transfer into our compatible memory DC.
					if (!BitBlt(
						hdcMemDC,
						dx, dy,
						w, h,
						hdcScreen,
						x1, y1,
						SRCCOPY))
					{
						MessageBox(_hwnd, L"BitBlt has failed", L"Failed", MB_OK);
						goto done;
					}
				}

				// Get the BITMAP from the HBITMAP
				GetObject(hbmScreen, sizeof(BITMAP), &bmpScreen);

				BITMAPFILEHEADER   bmfHeader;
				BITMAPINFOHEADER   bi;

				bi.biSize = sizeof(BITMAPINFOHEADER);
				bi.biWidth = bmpScreen->bmWidth;
				bi.biHeight = bmpScreen->bmHeight;
				bi.biPlanes = 1;
				bi.biBitCount = 32;
				bi.biCompression = BI_RGB;
				bi.biSizeImage = 0;
				bi.biXPelsPerMeter = 0;
				bi.biYPelsPerMeter = 0;
				bi.biClrUsed = 0;
				bi.biClrImportant = 0;

				DWORD dwBmpSize = ((bmpScreen->bmWidth * bi.biBitCount + 31) / 32) * 4 * bmpScreen->bmHeight;

				// Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that 
				// call HeapAlloc using a handle to the process's default heap. Therefore, GlobalAlloc and LocalAlloc 
				// have greater overhead than HeapAlloc.
				HANDLE hDIB = GlobalAlloc(GHND, dwBmpSize);
				char *lpbitmap = (char *)GlobalLock(hDIB);

				// If scaling
				if (_scalling)
				{
					// Gets the "bits" from the bitmap and copies them into a buffer 
					// which is pointed to by lpbitmap.
					GetDIBits(hdcWindow, hbmScreen, 0,
						(UINT)bmpScreen->bmHeight,
						lpbitmap,
						(BITMAPINFO *)&bi, DIB_RGB_COLORS);
				}
				else
				{
					// Gets the "bits" from the bitmap and copies them into a buffer 
					// which is pointed to by lpbitmap.
					GetDIBits(hdcScreen, hbmScreen, 0,
						(UINT)bmpScreen->bmHeight,
						lpbitmap,
						(BITMAPINFO *)&bi, DIB_RGB_COLORS);
				}

				// Assign the bit map bits.
				bmpScreen->bmBits = lpbitmap;

				//Unlock and Free the DIB from the heap
				GlobalUnlock(hDIB);
				GlobalFree(hDIB);

				//Clean up
			done:
				DeleteObject(hbmScreen);
				DeleteObject(hdcMemDC);
				ReleaseDC(NULL, hdcWindow);
				ReleaseDC(NULL, hdcScreen);

				return 0;
			}

			/// <summary>
			/// Set the scalling size.
			/// </summary>
			/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
			/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
			/// <param name="hwndScalling">The scalling capture handler.</param>
			void ScreenCapture::Scale(int nWidth, int nHeight, HWND hwndScalling)
			{
				_scallingWidth = nWidth;
				_scallingHeight = nHeight;
				_hwndScalling = hwndScalling;
			}

			/// <summary>
			/// Get the scalling indicator.
			/// </summary>
			/// <return>True if scalling is on; else false.</return>
			bool ScreenCapture::GetIsScalling()
			{
				return _scalling;
			}

			/// <summary>
			/// Set the scalling indicator.
			/// </summary>
			/// <param name="scalling">True if scalling is on; else false.</param>
			void ScreenCapture::SetIsScalling(bool scalling)
			{
				_scalling = scalling;
			}
		}
	}
}