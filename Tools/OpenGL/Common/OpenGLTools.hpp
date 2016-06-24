// This source file is part of OpenGL Code Samples
// For the latest info, see http://es.g0dsoft.com/
// Copyright (c) 2010 Michal Ziulek
// This source is under MIT License

#ifndef __opengl_tools_hpp__
#define __opengl_tools_hpp__

#include "opengl.hpp"
#include <string>

namespace gl {

/** Compiles GLSL shader. */
GLuint CompileShader(GLenum         type,
                     GLsizei        count,
                     const GLchar** source);

/** Links GLSL program. */
void LinkProgram(GLuint program);

/** Builds complete GLSL program from vertex and fragment shader sources. */
GLuint BuildProgram(const GLchar* vshSource,
                    const GLchar* fshSource,
                    bool          link = true);

/** Builds complete GLSL program from vertex, fragment and geometry shader sources. */
GLuint BuildProgram(const GLchar* vshSource,
                    const GLchar* gshSource,
                    const GLchar* fshSource,
                    bool          link = true);

/** Functions for getting OpenGL state. */
std::string GetShaderSource(GLuint shader);
std::string GetProgramSource(GLuint program);

} // namespace gl

#endif