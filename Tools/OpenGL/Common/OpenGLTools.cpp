// This source file is part of OpenGL Code Samples
// For the latest info, see http://es.g0dsoft.com/
// Copyright (c) 2010 Michal Ziulek
// This source is under MIT License

#include "OpenGLTools.hpp"
#include "SDL.h"
#include <sstream>
#include <stdexcept>
#include <vector>
#include <cassert>
using namespace std;

namespace {

const char* ShaderTypeToString(GLenum type) {
    switch (type) {
    case GL_VERTEX_SHADER: return "GL_VERTEX_SHADER";
    case GL_FRAGMENT_SHADER: return "GL_FRAGMENT_SHADER";
    case GL_GEOMETRY_SHADER: return "GL_GEOMETRY_SHADER";
    }

    assert(false);
    return NULL;
}

} // namespace

namespace gl {

//-----------------------------------------------------------------------------
GLuint CompileShader(GLenum type, GLsizei count, const GLchar** source) {
    const GLuint name = glCreateShader(type);

    try {
        glShaderSource(name, count, source, NULL);
        glCompileShader(name);

        GLint status;
        glGetShaderiv(name, GL_COMPILE_STATUS, &status);

        if (!status) {
            GLint s;
            glGetShaderiv(name, GL_INFO_LOG_LENGTH, &s);

            vector<GLchar> log(s);
            glGetShaderInfoLog(name, static_cast<GLsizei>(log.size()), NULL, &log[0]);
            
            ostringstream os;
            os <<
                "\n-----------------------------------------\n" <<
                "GLSL Compile Error" <<
                "\n-----------------------------------------\n\n" << &log[0] << GetShaderSource(name);

            throw runtime_error(os.str());
        }
    }
    catch (...) {
        glDeleteShader(name);
        throw;
    }

    return name;
}
//-----------------------------------------------------------------------------
void LinkProgram(GLuint name) {
    glLinkProgram(name);

    GLint status;
    glGetProgramiv(name, GL_LINK_STATUS, &status);

    if (!status) {
        GLint s;
        glGetProgramiv(name, GL_INFO_LOG_LENGTH, &s);
        
        vector<char> log(s);
        glGetProgramInfoLog(name, static_cast<GLsizei>(log.size()), NULL, &log[0]);

        ostringstream os;
        os <<
            "\n-----------------------------------------\n" <<
            "GLSL Link Error" <<
            "\n-----------------------------------------\n\n" << &log[0] << GetProgramSource(name);

        throw runtime_error(os.str());
    }
}
//-----------------------------------------------------------------------------
GLuint BuildProgram(const GLchar* vshSrc, const GLchar* fshSrc, bool link) {
    const GLuint p = glCreateProgram();

    try {
        if (vshSrc) {
            const GLuint sh = CompileShader(GL_VERTEX_SHADER, 1, &vshSrc);
            glAttachShader(p, sh);
            glDeleteShader(sh);
        }
        if (fshSrc) {
            const GLuint sh = CompileShader(GL_FRAGMENT_SHADER, 1, &fshSrc);
            glAttachShader(p, sh);
            glDeleteShader(sh);
        }
        if (link) {
            LinkProgram(p);
        }
    }
    catch (...) {
        glDeleteProgram(p);
        throw;
    }

    return p;
}
//-----------------------------------------------------------------------------
GLuint BuildProgram(const GLchar* vshSrc, const GLchar* gshSrc, const GLchar* fshSrc, bool link) {
    const GLuint p = glCreateProgram();

    try {
        if (vshSrc) {
            const GLuint sh = CompileShader(GL_VERTEX_SHADER, 1, &vshSrc);
            glAttachShader(p, sh);
            glDeleteShader(sh);
        }
        if (gshSrc) {
            const GLuint sh = CompileShader(GL_GEOMETRY_SHADER, 1, &gshSrc);
            glAttachShader(p, sh);
            glDeleteShader(sh);
        }
        if (fshSrc) {
            const GLuint sh = CompileShader(GL_FRAGMENT_SHADER, 1, &fshSrc);
            glAttachShader(p, sh);
            glDeleteShader(sh);
        }
        if (link) {
            LinkProgram(p);
        }
    }
    catch (...) {
        glDeleteProgram(p);
        throw;
    }

    return p;
}
//-----------------------------------------------------------------------------
string GetShaderSource(GLuint shader) {
    GLint sourceLength;
    glGetShaderiv(shader, GL_SHADER_SOURCE_LENGTH, &sourceLength);

    vector<char> source(sourceLength);
    glGetShaderSource(shader, static_cast<GLsizei>(source.size()), NULL, &source[0]);

    GLint type;
    glGetShaderiv(shader, GL_SHADER_TYPE, &type);

    ostringstream os;
    os <<
        "\n" <<
        "//-----------------------------------------\n" <<
        "// " << ShaderTypeToString(type) << "\n" <<
        "//-----------------------------------------\n" << &source[0] << "\n";

    return os.str();
}
//-----------------------------------------------------------------------------
string GetProgramSource(GLuint program) {
    GLint shaderCount;
    glGetProgramiv(program, GL_ATTACHED_SHADERS, &shaderCount);

    if (shaderCount > 0) {
        vector<GLuint> shaders(shaderCount);
        glGetAttachedShaders(program, static_cast<GLsizei>(shaders.size()), NULL, &shaders[0]);

        ostringstream os;
        for (size_t i = 0; i < shaders.size(); ++i) {
            os << GetShaderSource(shaders[i]);
        }

        return os.str();
    }

    return "";
}
//-----------------------------------------------------------------------------

} // namespace gl