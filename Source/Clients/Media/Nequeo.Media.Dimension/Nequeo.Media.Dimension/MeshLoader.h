/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MeshLoader.h
*  Purpose :       Mesh loader class.
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

#ifndef _MESHLOADER_H
#define _MESHLOADER_H

#include "stdafx.h"

#include "GL\glew.h"
#include "glm\glm.hpp"

namespace Nequeo
{
	namespace Media
	{
		namespace Dimension
		{
			///	<summary>
			///	3D mesh loader class.
			///	</summary>
			class MeshLoader
			{
			public:
				///	<summary>
				///	3D mesh loader.
				///	</summary>
				MeshLoader();

				///	<summary>
				///	3D mesh loader.
				///	</summary>
				~MeshLoader();

				/// <summary>
				/// Load the mesh data from file.
				/// </summary>
				/// <param name="filename">The file and path where the mesh data is to be loaded from.</param>
				void Load(const char* filename);

				/// <summary>
				/// Get the vertex size.
				/// </summary>
				/// <returns>The open GL size.</returns>
				GLsizei VertexSize() const;

				/// <summary>
				/// Get the vertex count.
				/// </summary>
				/// <returns>The open GL count.</returns>
				GLsizei VertexCount() const;

				/// <summary>
				/// Get the index count.
				/// </summary>
				/// <returns>The open GL count.</returns>
				GLsizei IndexCount() const;

				/// <summary>
				/// Get the vertex data.
				/// </summary>
				/// <returns>The open GL mathematics vector data.</returns>
				const glm::vec3* VertexData() const;

				/// <summary>
				/// Get the index data.
				/// </summary>
				/// <returns>The open GL mathematics index data.</returns>
				const glm::uint* IndexData() const;

			private:
				bool _disposed;

				std::vector<glm::vec3> _vertexData;
				std::vector<glm::uint> _indexData;
			};
		}
	}
}
#endif