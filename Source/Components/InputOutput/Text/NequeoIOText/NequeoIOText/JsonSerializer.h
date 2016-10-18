/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          JsonSerializer.h
*  Purpose :       JsonSerializer header.
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

#ifndef _JSONSERIALIZER_H
#define _JSONSERIALIZER_H

#include "GlobalText.h"

#include "Base\Array.h"
#include "Base\Allocator.h"

#include <json/json.h>

namespace Nequeo {
	namespace IO {
		namespace JSON
		{
			/**
			* Json Document tree object that supports parsing and serialization.
			*/
			class JsonSerializer
			{
			public:
				/**
				* Constructs empty json object
				*/
				JsonSerializer();

				/**
				* Constructs a json object from a json string
				*/

				JsonSerializer(const Nequeo::String& value);

				/**
				* Constructs a json object from a stream containing json
				*/
				JsonSerializer(Nequeo::IStream& istream);

				/**
				* Copy Constructor
				*/
				JsonSerializer(const JsonSerializer& value);

				/**
				* Move Constructor
				*/
				JsonSerializer(JsonSerializer&& value);

				~JsonSerializer();

				JsonSerializer& operator=(const JsonSerializer& other);

				JsonSerializer& operator=(JsonSerializer&& other);

				bool operator!=(const JsonSerializer& other)
				{
					return m_value != other.m_value;
				}

				bool operator==(const JsonSerializer& other)
				{
					return m_value == other.m_value;
				}

				/**
				* Gets a string from the top level of this node by it's key
				*/
				std::string GetString(const Nequeo::String& key) const;
				std::string GetString(const char* key) const;

				/**
				* Adds a string to the top level of this node with key
				*/
				JsonSerializer& WithString(const Nequeo::String& key, const std::string& value);
				JsonSerializer& WithString(const char* key, const std::string& value);

				/**
				* Causes the json node to be a string only (makes it a leaf node in token tree)
				*/
				JsonSerializer& AsString(const std::string& value);

				/**
				* Returns the value of this node as a string as if it was a leaf node in the token tree
				*/
				Nequeo::String AsString() const;

				/**
				* Gets a bool value from the top level of this node by its key.
				*/
				bool GetBool(const Nequeo::String& key) const;
				bool GetBool(const char* key) const;

				/**
				* Adds a bool value with key to the top level of this node.
				*/
				JsonSerializer& WithBool(const Nequeo::String& key, bool value);
				JsonSerializer& WithBool(const char* key, bool value);

				/**
				* Sets this node to be a bool (makes it a leaf node in the token tree)
				*/
				JsonSerializer& AsBool(bool value);

				/**
				* Gets the value of this node as a bool
				*/
				bool AsBool() const;

				/**
				* Gets the integer value at key on the top level of this node.
				*/
				int GetInteger(const Nequeo::String& key) const;
				int GetInteger(const char* key) const;

				/**
				* Adds an integer value at key at the top level of this node.
				*/
				JsonSerializer& WithInteger(const Nequeo::String& key, int value);
				JsonSerializer& WithInteger(const char* key, int value);

				/**
				* Causes this node to be an integer (becomes a leaf node in the token tree).
				*/
				JsonSerializer& AsInteger(int value);

				/**
				* Gets the integer value from a leaf node.
				*/
				int AsInteger() const;

				/**
				* Gets the 64 bit integer value at key from the top level of this node.
				*/
				long long GetInt64(const Nequeo::String& key) const;
				long long GetInt64(const char* key) const;

				/**
				* Adds a 64 bit integer value at key to the top level of this node.
				*/
				JsonSerializer& WithInt64(const Nequeo::String& key, long long value);
				JsonSerializer& WithInt64(const char* key, long long value);

				/**
				* Causes this node to be interpreted as a 64 bit integer (becomes treated like a leaf node).
				*/
				JsonSerializer& AsInt64(long long value);

				/**
				* Gets the value of this node as a 64bit integer.
				*/
				long long AsInt64() const;

				/**
				* Gets the value of a double at key at the top level of this node.
				*/
				double GetDouble(const Nequeo::String& key) const;
				double GetDouble(const char* key) const;

				/**
				* Adds a double value at key at the top level of this node.
				*/
				JsonSerializer& WithDouble(const Nequeo::String& key, double value);
				JsonSerializer& WithDouble(const char* key, double value);

				/**
				* Causes this node to be interpreted as a double value.
				*/
				JsonSerializer& AsDouble(double value);

				/**
				* Gets the value of this node as a double.
				*/
				double AsDouble() const;

				/**
				* Gets an array from the top level of this node at key.
				*/
				Array<JsonSerializer> GetArray(const Nequeo::String& key) const;
				Array<JsonSerializer> GetArray(const char* key) const;

				/**
				* Adds an array of strings to the top level of this node at key.
				*/
				JsonSerializer& WithArray(const Nequeo::String& key, const Array<Nequeo::String>& array);
				JsonSerializer& WithArray(const char* key, const Array<Nequeo::String>& array);

				/**
				* Adds an array of strings to the top level of this node at key. Array will be unusable after this call
				* only use if you intend this to be an r-value. Better yet, let the compiler make this decision
				* for you, but if you must.... std::move will do the trick.
				*/
				JsonSerializer& WithArray(const Nequeo::String& key, Array<Nequeo::String>&& array);

				/**
				* Adds an array of arbitrary json objects to the top level of this node at key.
				*/
				JsonSerializer& WithArray(const Nequeo::String& key, const Array<JsonSerializer>& array);

				/**
				* Adds an array of arbitrary json objects to the top level of this node at key. Array will be unusable after this call
				* only use if you intend this to be an r-value. Better yet, let the compiler make this decision
				* for you, but if you must.... std::move will do the trick.
				*/
				JsonSerializer& WithArray(const Nequeo::String& key, Array<JsonSerializer>&& array);

				/**
				* Causes this node to be interpreted as an array.
				*/
				JsonSerializer& AsArray(const Array<JsonSerializer>& array);

				/**
				* Causes this node to be interpreted as an array using move semantics on array.
				*/
				JsonSerializer& AsArray(Array<JsonSerializer>&& array);

				/**
				* Interprets this node as an array and returns a copy of it's values.
				*/
				Array<JsonSerializer> AsArray() const;

				/**
				* Gets a json object from the top level of this node at key.
				*/
				JsonSerializer GetObject(const char* key) const;
				JsonSerializer GetObject(const Nequeo::String& key) const;

				/**
				* Adds a json object to the top level of this node at key.
				*/
				JsonSerializer& WithObject(const Nequeo::String& key, const JsonSerializer& value);
				JsonSerializer& WithObject(const char* key, const JsonSerializer& value);

				/**
				* Adds a json object to the top level of this node at key using move semantics.
				*/
				JsonSerializer& WithObject(const Nequeo::String& key, const JsonSerializer&& value);
				JsonSerializer& WithObject(const char* key, const JsonSerializer&& value);

				/**
				* Causes this node to be interpreted as another json object
				*/
				JsonSerializer& AsObject(const JsonSerializer& value);

				/**
				* Causes this node to be interpreted as another json object using move semantics.
				*/
				JsonSerializer& AsObject(JsonSerializer&& value);

				/**
				* Gets the value of this node as a json object.
				*/
				JsonSerializer AsObject() const;

				/**
				* Reads all json objects at the top level of this node (does not traverse the tree any further)
				* along with their keys.
				*/
				Nequeo::Map<Nequeo::String, JsonSerializer> GetAllObjects() const;

				/**
				* Whether or not a value exists at the current node level at a given key.
				*
				* Returns true if a values has been found, false otherwise.
				*/
				bool ValueExists(const char* key) const;
				bool ValueExists(const Nequeo::String& key) const;

				/**
				* Writes the entire json object tree without whitespace characters starting at the current level to a string and
				* returns it.
				*/
				std::string WriteCompact(bool treatAsObject = true) const;

				/**
				* Writes the entire json object tree to ostream without whitespace characters at the current level.
				*/
				void WriteCompact(Nequeo::OStream& ostream, bool treatAsObject = true) const;

				/**
				* Writes the entire json object tree starting at the current level to a string and
				* returns it.
				*/
				std::string WriteReadable(bool treatAsObject = true) const;

				/**
				* Writes the entire json object tree to ostream at the current level.
				*/
				void WriteReadable(Nequeo::OStream& ostream, bool treatAsObject = true) const;

				/**
				* Returns true if the last parse request was successful. If this returns false,
				* you can call GetErrorMessage() to find the cause.
				*/
				inline bool WasParseSuccessful() const
				{
					return m_wasParseSuccessful;
				}

				/**
				* Returns the last error message from a failed parse attempt. Returns empty string if no error.
				*/
				inline const std::string& GetErrorMessage() const
				{
					return m_errorMessage;
				}
				/**
				* Appends a json object as a child to the end of this object
				*/
				void AppendValue(const JsonSerializer& value);

				bool IsObject() const;
				bool IsBool() const;
				bool IsString() const;
				bool IsIntegerType() const;
				bool IsFloatingPointType() const;
				bool IsListType() const;

				Json::Value& ModifyRawValue() { return m_value; }

			private:
				JsonSerializer(const Json::Value& value);

				JsonSerializer& operator=(Json::Value& other);

				mutable Json::Value m_value;
				bool m_wasParseSuccessful;
				std::string m_errorMessage;
			};
		}
	}
}
#endif
