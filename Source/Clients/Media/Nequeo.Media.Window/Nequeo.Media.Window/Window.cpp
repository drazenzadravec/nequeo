/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Window.cpp
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

#include "stdafx.h"

#include "Window.h"

using namespace Nequeo::Media;

///	<summary>
///	GUI window generator.
///	</summary>
/// <param name="title">The window title.</param>
/// <param name="width">The window width.</param>
/// <param name="height">The window height.</param>
/// <param name="xPosition">The x window position.</param>
/// <param name="yPosition">The y window position.</param>
Window::Window(const char* title, int width, int height, int xPosition, int yPosition) : _disposed(false)
{
	SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
	SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_ALPHA_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
	SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);

	// Create the window.
	_window = SDL_CreateWindow(title, xPosition, yPosition, width, height, SDL_WINDOW_SHOWN | SDL_WINDOW_OPENGL);
	if (!_window) 
	{
		// If the window was not created.
		throw std::runtime_error("Create window failed");
	}

	_width = width;
	_height = height;
	_lastTime = SDL_GetTicks();
	_lastSecond = _lastTime;
	_frameCount = 0;
}

///	<summary>
///	GUI window generator.
///	</summary>
Window::~Window()
{
	if (!_disposed)
	{
		_disposed = true;

		// Clean-up the current window.
		SDL_DestroyWindow(_window);
		_window = NULL;
	}
}

///	<summary>
///	Swaps OpenGL buffers.
///	</summary>
void Window::Swap()
{
	_frameCount++;

	// Get the current window ticks.
	const Uint32 now = SDL_GetTicks();
	const Uint32 dt = now - _lastTime;
	_lastTime = now;

	// If 1000 mini-seconds has elasped.
	if (now - _lastSecond > 1000) 
	{
		// Get the current frames per second calculation.
		const float fps = static_cast<float>(_frameCount) / (now - _lastSecond) * 1000.0f;

		// Get the window title.
		std::string name(SDL_GetWindowTitle(_window));
		size_t b = name.find(" - ");
		const size_t e = name.find(" [");

		// If string exists.
		if (b != std::string::npos && e != std::string::npos)
		{
			b += 3;

			// Get the sub string.
			name = name.substr(b, e - b);
		}

		// Load the string data into the string stream.
		std::ostringstream os;
		os << "OpenGL Code Sample - " << name << " [fps: " << fps << ", ms: " <<
			(now - _lastSecond) / static_cast<float>(_frameCount) << "]";

		// Change the window title.
		SDL_SetWindowTitle(_window, os.str().c_str());

		_lastSecond = now;
		_frameCount = 0;
	}

	// Swap the window.
	SDL_GL_SwapWindow(_window);
}

/// <summary>
/// Get the current window.
/// </summary>
/// <returns>The window.</returns>
SDL_Window* Window::Get() const
{
	return _window;
}

/// <summary>
/// Get the window width.
/// </summary>
/// <returns>The window width.</returns>
int Window::Width() const
{
	return _width;
}

/// <summary>
/// Get the window height.
/// </summary>
/// <returns>The window height.</returns>
int Window::Height() const
{
	return _height;
}