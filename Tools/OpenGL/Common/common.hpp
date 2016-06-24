// This source file is part of OpenGL Code Samples
// For the latest info, see http://es.g0dsoft.com/
// Copyright (c) 2010 Michal Ziulek
// This source is under MIT License

#ifndef __common_hpp__
#define __common_hpp__

#include "SDL.h"

#include "OpenGL.hpp"
#include "OpenGLTools.hpp"
#include "OpenGLMath.hpp"

#include "AntTweakBar.h"

#include "stb_image.h"
#include "stb_image_write.h"

#include "glm/glm.hpp"

// on windows SDL defines main as SDL_main
#if defined(__WIN32__) && defined(main)
#   undef main
#endif

/** Helper string macros. */
#define __OGL_STRINGIFY(_x) #_x
#define _OGL_STRINGIFY(_x) __OGL_STRINGIFY(_x)
#define OGL_SHADER_BEGIN(ver) "#version " _OGL_STRINGIFY(ver) "\n#line " _OGL_STRINGIFY(__LINE__) " /* " _OGL_STRINGIFY(__FILE__) " */ "

/** Handles common window events. Returns true if event was processed. */
bool ProcessCommonEvent(const SDL_Event& evt);
bool ProcessAntTweakBarEvent(const SDL_Event& evt);

/** Displays error message for the user. */
void DisplayErrorMessage(const char* header, const char* msg);

/**
 * Handles OpenGL window.
 */
class Window {
public:
    Window(const char* title, int w, int h, int x = SDL_WINDOWPOS_UNDEFINED, int y = SDL_WINDOWPOS_UNDEFINED);
    ~Window();

    /** Swaps OpenGL buffers. */
    void Swap();

    SDL_Window* Get() const { return mWindow; }
    int Width() const { return mWidth; }
    int Height() const { return mHeight; }

private:
    SDL_Window* mWindow;
    int mWidth, mHeight;
    int mFrameCount;
    Uint32 mLastTime, mLastSecond;
};

/**
 * Rotates 3D object based on user's input.
 */
class ArcBall {
public:
    ArcBall() : mWidth(0), mHeight(0) {}

    void Begin(int x, int y);
    void End();
    void Drag(int x, int y);
    void SetWindowSize(int width, int height);
    const glm::mat3 Rotation() const { return glm::mat3_cast(mQuatNow); }

private:
    glm::vec3 ScreenToVector(float screenX, float screenY);

    int mWidth, mHeight;
    bool mDragging;
    glm::quat mQuatNow, mQuatDown;
    glm::vec3 mVecNow, mVecDown;
};

#endif