// This source file is part of OpenGL Code Samples
// For the latest info, see http://es.g0dsoft.com/
// Copyright (c) 2010 Michal Ziulek
// This source is under MIT License


#include "Common.hpp"
#include <sstream>
#include <iostream>
#include <vector>
#include <algorithm>
using namespace std;

#if defined(__WIN32__) && defined(APIENTRY)
#   undef APIENTRY
#endif
#include "SDL_syswm.h"

//-----------------------------------------------------------------------------
bool ProcessCommonEvent(const SDL_Event& evt) {
    switch (evt.type) {
    case SDL_WINDOWEVENT:
        if (evt.window.event == SDL_WINDOWEVENT_CLOSE) { // quit
            SDL_Event evt = { 0 };
            evt.type = SDL_QUIT;
            SDL_PushEvent(&evt);
            return true;
        }
        break;

    case SDL_KEYDOWN:
        if (evt.key.keysym.sym == SDLK_ESCAPE) { // quit
            SDL_Event evt = { 0 };
            evt.type = SDL_QUIT;
            SDL_PushEvent(&evt);
            return true;
        }
        else if (evt.key.keysym.sym == SDLK_F12) { // make a screenshot
            SDL_Window* window = SDL_GetWindowFromID(evt.key.windowID);
            if (window) {
                static int number = 0;
                ostringstream ss;
                ss << "screenshot" << number++ << ".png";

                int w, h;
                SDL_GetWindowSize(window, &w, &h);
                const int stride = w * 3;
                vector<unsigned char> fb(h * stride);
                glReadPixels(0, 0, w, h, GL_RGB, GL_UNSIGNED_BYTE, &fb[0]);

                vector<unsigned char> fbFlipped(fb.size());
                for (int y = 0; y < h; ++y) {
                    copy(fb.begin() + y * stride, fb.begin() + y * stride + stride, fbFlipped.end() - (y + 1) * stride);
                }

                stbi_write_png(ss.str().c_str(), w, h, 3, &fbFlipped[0], stride);
                return true;
            }
        }
        break;
    }

    return false;
}
//-----------------------------------------------------------------------------
bool ProcessAntTweakBarEvent(const SDL_Event& evt) {
    switch (evt.type) {
    case SDL_MOUSEBUTTONUP:
    case SDL_MOUSEBUTTONDOWN:
        return TwMouseButton(
            evt.type == SDL_MOUSEBUTTONUP ? TW_MOUSE_RELEASED : TW_MOUSE_PRESSED,
            static_cast<TwMouseButtonID>(evt.button.button)) != 0;

    case SDL_MOUSEMOTION:
        return TwMouseMotion(evt.motion.x, evt.motion.y) != 0;
    }

    return false;
}
//-----------------------------------------------------------------------------
void DisplayErrorMessage(const char* header, const char* msg) {
    cout << "[" << header << "] " << msg << endl;

#if defined(__WIN32__)

	wchar_t* wideMSG = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, msg, -1, wideMSG, 4096);

	wchar_t* wideHeader = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, header, -1, wideHeader, 4096);

    MessageBox(NULL, wideMSG, wideHeader, MB_OK | MB_ICONERROR);
#endif
}
//-----------------------------------------------------------------------------
// Window
//-----------------------------------------------------------------------------
Window::Window(const char* title, int w, int h, int x, int y) {
    SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
    SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 8);
    SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 8);
    SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 8);
    SDL_GL_SetAttribute(SDL_GL_ALPHA_SIZE, 8);
    SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
    SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);

    mWindow = SDL_CreateWindow(title, x, y, w, h, SDL_WINDOW_SHOWN | SDL_WINDOW_OPENGL);
    if (!mWindow) {
        throw runtime_error("SDL_CreateWindow failed");
    }

    mWidth = w;
    mHeight = h;

    mLastTime = SDL_GetTicks();
    mLastSecond = mLastTime;
    mFrameCount = 0;
}
//-----------------------------------------------------------------------------
Window::~Window() {
    SDL_DestroyWindow(mWindow);
}
//-----------------------------------------------------------------------------
void Window::Swap() {
    mFrameCount++;

    const Uint32 now = SDL_GetTicks();
    const Uint32 dt = now - mLastTime;
    mLastTime = now;

    if (now - mLastSecond > 1000) {
        const float fps = static_cast<float>(mFrameCount) / (now - mLastSecond) * 1000.0f;

        string name(SDL_GetWindowTitle(mWindow));
        size_t b = name.find(" - ");
        const size_t e = name.find(" [");
        if (b != string::npos && e != string::npos) {
            b += 3;
            name = name.substr(b, e - b);
        }

        ostringstream os;
        os << "OpenGL Code Sample - " << name << " [fps: " << fps << ", ms: " <<
            (now - mLastSecond) / static_cast<float>(mFrameCount) << "]";
        SDL_SetWindowTitle(mWindow, os.str().c_str());

        mLastSecond = now;
        mFrameCount = 0;
    }

    SDL_GL_SwapWindow(mWindow);
}
//-----------------------------------------------------------------------------
// ArcBall
//-----------------------------------------------------------------------------
void ArcBall::Begin(int x, int y) {
    mDragging = true;
    mVecDown = ScreenToVector(static_cast<float>(x), static_cast<float>(y));
    mQuatDown = mQuatNow;
}
//-----------------------------------------------------------------------------
void ArcBall::End() {
    mDragging = false;
}
//-----------------------------------------------------------------------------
void ArcBall::Drag(int x, int y) {
    if (mDragging) {
        mVecNow = ScreenToVector(static_cast<float>(x), static_cast<float>(y));

        glm::vec3 p = glm::cross(mVecDown, mVecNow);

        if (glm::length(p) > 1e-5) {
            glm::quat q = glm::quat(glm::dot(mVecDown, mVecNow), p);
            mQuatNow = glm::cross(glm::normalize(q), mQuatDown);
        }
        else {
            mQuatNow = glm::cross(glm::quat(), mQuatDown);
        }
    }
}
//-----------------------------------------------------------------------------
void ArcBall::SetWindowSize(int width, int height) {
    mWidth = width;
    mHeight = height;
}
//-----------------------------------------------------------------------------
glm::vec3 ArcBall::ScreenToVector(float screenX, float screenY) {
    glm::vec2 v;
    v.x = ((screenX / ((mWidth - 1) / 2)) - 1);
    v.y = -((screenY / ((mHeight - 1) / 2)) - 1);

    float len = glm::length(v);
    if (len > 1.0f)
        return glm::vec3(v / sqrt(len), 0);

    return glm::vec3(v, sqrt(1.0f - len));
}
//-----------------------------------------------------------------------------