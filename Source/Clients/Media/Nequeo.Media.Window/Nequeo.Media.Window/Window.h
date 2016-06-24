/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Window.h
*  Purpose :       Window class.
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

#ifndef _WINDOW_H
#define _WINDOW_H

#include "stdafx.h"

#include "SDL.h"

namespace Nequeo
{
	namespace Media
	{
		///	<summary>
		///	GUI window generator class.
		///	</summary>
		class EXPORT_NEQUEO_MEDIA_WINDOW_API Window
		{
		public:
			///	<summary>
			///	GUI window generator.
			///	</summary>
			/// <param name="title">The window title.</param>
			/// <param name="width">The window width.</param>
			/// <param name="height">The window height.</param>
			/// <param name="xPosition">The x window position.</param>
			/// <param name="yPosition">The y window position.</param>
			Window(const char* title, int width, int height, int xPosition = SDL_WINDOWPOS_UNDEFINED, int yPosition = SDL_WINDOWPOS_UNDEFINED);

			///	<summary>
			///	GUI window generator.
			///	</summary>
			~Window();

			///	<summary>
			///	Swaps OpenGL buffers.
			///	</summary>
			void Swap();

			/// <summary>
			/// Get the current window.
			/// </summary>
			/// <returns>The window.</returns>
			SDL_Window* Get() const;

			/// <summary>
			/// Get the window width.
			/// </summary>
			/// <returns>The window width.</returns>
			int Width() const;

			/// <summary>
			/// Get the window height.
			/// </summary>
			/// <returns>The window height.</returns>
			int Height() const;

		private:
			bool _disposed;

			SDL_Window* _window;
			int _width;
			int _height;
			int _frameCount;
			Uint32 _lastTime;
			Uint32 _lastSecond;
		};
	}
}
#endif