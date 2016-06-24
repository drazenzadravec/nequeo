/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MeshLoader.cpp
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

#include "stdafx.h"

#include "MeshLoader.h"

using namespace Nequeo::Media::Dimension;

///	<summary>
///	3D mesh loader.
///	</summary>
MeshLoader::MeshLoader() : _disposed(false)
{
}

///	<summary>
///	3D mesh loader.
///	</summary>
MeshLoader::~MeshLoader()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Load the mesh data from file.
/// </summary>
/// <param name="filename">The file and path where the mesh data is to be loaded from.</param>
void MeshLoader::Load(const char* filename)
{
	// Open the file for reading.
	FILE* fp = fopen(filename, "r");

	try 
	{
		if (!fp) 
		{
			// If file can not be opened.
			throw std::runtime_error(std::string("failed to load ") + filename);
		}

		char command[256];
		int vertexCount = -1, faceCount = -1;

		// While true load header data.
		while (true) 
		{
			fscanf(fp, "%s", &command[0]);

			if (feof(fp) || strcmp(command, "end_header") == 0) 
			{
				break;
			}
			else if (strcmp(command, "element") == 0) 
			{
				fscanf(fp, "%s", &command[0]);
				if (strcmp(command, "vertex") == 0) 
				{
					fscanf(fp, "%d", &vertexCount);
				}
				else if (strcmp(command, "face") == 0) 
				{
					fscanf(fp, "%d", &faceCount);
				}
			}

			// Get the command.
			fgets(&command[0], sizeof(command), fp);
		}

		if (vertexCount < 0 || faceCount < 0) 
		{
			// If no vertex data and no face data.
			throw std::runtime_error(std::string("failed to load ") + filename);
		}

		_vertexData.resize(vertexCount * 2);

		// Read all the mesh vertex data.
		for (int i = 0; i < vertexCount; ++i) 
		{
			glm::vec3 v;
			fscanf(fp, "%f %f %f", &v.x, &v.z, &v.y);
			v.z = -v.z;
			_vertexData[i * 2] = v;

			fscanf(fp, "%f %f %f", &v.x, &v.z, &v.y);
			v.z = -v.z;
			_vertexData[i * 2 + 1] = v;
		}

		int c;
		_indexData.resize(faceCount * 3);

		// Read the face count and index data.
		for (int i = 0; i < faceCount; ++i) 
		{
			const int idx = i * 3;
			fscanf(fp, "%d %d %d %d", &c, &_indexData[idx], _indexData[idx + 1], &_indexData[idx + 2]);
		}
	}
	catch (...) {
		if (fp) 
		{
			// Close the file.
			fclose(fp);
		}
		throw;
	}

	// Close the file.
	fclose(fp);
}

/// <summary>
/// Get the vertex size.
/// </summary>
/// <returns>The open GL size.</returns>
GLsizei MeshLoader::VertexSize() const
{
	return static_cast<GLsizei>(sizeof(glm::vec3) * 2);
}

/// <summary>
/// Get the vertex count.
/// </summary>
/// <returns>The open GL count.</returns>
GLsizei MeshLoader::VertexCount() const
{
	return static_cast<GLsizei>(_vertexData.size() / 2);
}

/// <summary>
/// Get the index count.
/// </summary>
/// <returns>The open GL count.</returns>
GLsizei MeshLoader::IndexCount() const
{
	return static_cast<GLsizei>(_indexData.size());
}

/// <summary>
/// Get the vertex data.
/// </summary>
/// <returns>The open GL mathematics vector data.</returns>
const glm::vec3* MeshLoader::VertexData() const
{
	return &_vertexData[0];
}

/// <summary>
/// Get the index data.
/// </summary>
/// <returns>The open GL mathematics index data.</returns>
const glm::uint* MeshLoader::IndexData() const
{
	return &_indexData[0];
}