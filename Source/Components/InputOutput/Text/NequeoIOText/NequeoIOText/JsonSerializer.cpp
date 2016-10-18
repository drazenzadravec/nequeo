/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          JsonSerializer.cpp
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

#include "stdafx.h"

#include "JsonSerializer.h"

using namespace Json;

namespace Nequeo {
	namespace IO {
		namespace JSON
		{
			JsonSerializer::JsonSerializer() : m_wasParseSuccessful(true)
			{
			}

			JsonSerializer::JsonSerializer(const Nequeo::String& value) : m_wasParseSuccessful(true)
			{
				Json::Reader reader;
				std::string strValue(value.c_str());

				if (!reader.parse(strValue, m_value))
				{
					m_wasParseSuccessful = false;
					m_errorMessage = reader.getFormattedErrorMessages();
				}
			}

			JsonSerializer::JsonSerializer(Nequeo::IStream& istream) : m_wasParseSuccessful(true)
			{
				Json::Reader reader;

				if (!reader.parse(istream, m_value))
				{
					m_wasParseSuccessful = false;
					m_errorMessage = reader.getFormattedErrorMessages();
				}
			}

			JsonSerializer::JsonSerializer(const JsonSerializer& value)
			{
				AsObject(value);
			}

			JsonSerializer::JsonSerializer(JsonSerializer&& value) : m_value(std::move(value.m_value))
			{
			}

			JsonSerializer::~JsonSerializer()
			{
			}

			JsonSerializer& JsonSerializer::operator=(const JsonSerializer& other)
			{
				if (this == &other)
				{
					return *this;
				}

				return AsObject(other);

			}

			JsonSerializer& JsonSerializer::operator=(JsonSerializer&& other)
			{
				if (this == &other)
				{
					return *this;
				}

				return AsObject(other);
			}

			JsonSerializer::JsonSerializer(const Json::Value& value)
			{
				m_value = value;
			}

			JsonSerializer& JsonSerializer::operator=(Json::Value& other)
			{
				m_value = other;
				return *this;
			}

			std::string JsonSerializer::GetString(const char* key) const
			{
				return m_value[key].asString();
			}

			std::string JsonSerializer::GetString(const Nequeo::String& key) const
			{
				return GetString(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithString(const char* key, const std::string& value)
			{
				m_value[key] = value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithString(const Nequeo::String& key, const std::string& value)
			{
				return WithString(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::AsString(const std::string& value)
			{
				m_value = value;
				return *this;
			}

			Nequeo::String JsonSerializer::AsString() const
			{
				Nequeo::String stringValue = m_value.asString().c_str();
				return stringValue;
			}

			bool JsonSerializer::GetBool(const char* key) const
			{
				return m_value[key].asBool();
			}

			bool JsonSerializer::GetBool(const Nequeo::String& key) const
			{
				return GetBool(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithBool(const char* key, bool value)
			{
				m_value[key] = value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithBool(const Nequeo::String& key, bool value)
			{
				return WithBool(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::AsBool(bool value)
			{
				m_value = value;
				return *this;
			}

			bool JsonSerializer::AsBool() const
			{
				return m_value.asBool();
			}

			int JsonSerializer::GetInteger(const char* key) const
			{
				return m_value[key].asInt();
			}

			int JsonSerializer::GetInteger(const Nequeo::String& key) const
			{
				return GetInteger(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithInteger(const char* key, int value)
			{
				m_value[key] = value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithInteger(const Nequeo::String& key, int value)
			{
				return WithInteger(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::AsInteger(int value)
			{
				m_value = value;
				return *this;
			}

			int JsonSerializer::AsInteger() const
			{
				return m_value.asInt();
			}

			long long JsonSerializer::GetInt64(const char* key) const
			{
				return m_value[key].asLargestInt();
			}

			long long JsonSerializer::GetInt64(const Nequeo::String& key) const
			{
				return GetInt64(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithInt64(const char* key, long long value)
			{
				m_value[key] = value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithInt64(const Nequeo::String& key, long long value)
			{
				return WithInt64(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::AsInt64(long long value)
			{
				m_value = value;
				return *this;
			}

			long long JsonSerializer::AsInt64() const
			{
				return m_value.asLargestInt();
			}

			double JsonSerializer::GetDouble(const char* key) const
			{
				return m_value[key].asDouble();
			}

			double JsonSerializer::GetDouble(const Nequeo::String& key) const
			{
				return GetDouble(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithDouble(const char* key, double value)
			{
				m_value[key] = value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithDouble(const Nequeo::String& key, double value)
			{
				return WithDouble(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::AsDouble(double value)
			{
				m_value = value;
				return *this;
			}

			double JsonSerializer::AsDouble() const
			{
				return m_value.asDouble();
			}

			Array<JsonSerializer> JsonSerializer::GetArray(const char* key) const
			{
				Array<JsonSerializer> returnArray(m_value[key].size());

				for (unsigned i = 0; i < returnArray.GetLength(); ++i)
				{
					returnArray[i] = m_value[key][i];
				}

				return returnArray;
			}

			Array<JsonSerializer> JsonSerializer::GetArray(const Nequeo::String& key) const
			{
				return GetArray(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithArray(const char* key, const Array<Nequeo::String>& array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
				{
					Json::Value stringValue(array[i].c_str());
					arrayValue.append(stringValue);
				}

				m_value[key] = arrayValue;

				return *this;
			}

			JsonSerializer& JsonSerializer::WithArray(const Nequeo::String& key, const Array<Nequeo::String>& array)
			{
				return WithArray(key.c_str(), array);
			}

			JsonSerializer& JsonSerializer::WithArray(const Nequeo::String& key, Array<Nequeo::String>&& array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
				{
					Json::Value stringValue(array[i].c_str());
					arrayValue.append(std::move(stringValue));
				}

				m_value[key.c_str()] = std::move(arrayValue);

				return *this;
			}

			JsonSerializer& JsonSerializer::WithArray(const Nequeo::String& key, const Array<JsonSerializer>& array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
					arrayValue.append(array[i].m_value);

				m_value[key.c_str()] = arrayValue;

				return *this;
			}

			JsonSerializer& JsonSerializer::WithArray(const Nequeo::String& key, Array<JsonSerializer>&& array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
					arrayValue.append(std::move(array[i].m_value));

				m_value[key.c_str()] = std::move(arrayValue);

				return *this;
			}

			void JsonSerializer::AppendValue(const JsonSerializer& value)
			{
				m_value.append(value.m_value);
			}

			JsonSerializer& JsonSerializer::AsArray(const Array<JsonSerializer>& array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
					arrayValue.append(array[i].m_value);

				m_value = arrayValue;
				return *this;
			}

			JsonSerializer& JsonSerializer::AsArray(Array<JsonSerializer> && array)
			{
				Json::Value arrayValue;
				for (unsigned i = 0; i < array.GetLength(); ++i)
					arrayValue.append(std::move(array[i].m_value));

				m_value = std::move(arrayValue);
				return *this;
			}

			Array<JsonSerializer> JsonSerializer::AsArray() const
			{
				Array<JsonSerializer> returnArray(m_value.size());

				for (unsigned i = 0; i < returnArray.GetLength(); ++i)
				{
					returnArray[i] = m_value[i];
				}

				return returnArray;
			}

			JsonSerializer JsonSerializer::GetObject(const char* key) const
			{
				return m_value[key];
			}

			JsonSerializer JsonSerializer::GetObject(const Nequeo::String& key) const
			{
				return GetObject(key.c_str());
			}

			JsonSerializer& JsonSerializer::WithObject(const char* key, const JsonSerializer& value)
			{
				m_value[key] = value.m_value;
				return *this;
			}

			JsonSerializer& JsonSerializer::WithObject(const Nequeo::String& key, const JsonSerializer& value)
			{
				return WithObject(key.c_str(), value);
			}

			JsonSerializer& JsonSerializer::WithObject(const char* key, const JsonSerializer&& value)
			{
				m_value[key] = std::move(value.m_value);
				return *this;
			}

			JsonSerializer& JsonSerializer::WithObject(const Nequeo::String& key, const JsonSerializer&& value)
			{
				return WithObject(key.c_str(), std::move(value));
			}

			JsonSerializer& JsonSerializer::AsObject(const JsonSerializer& value)
			{
				m_value = value.m_value;
				return *this;
			}

			JsonSerializer& JsonSerializer::AsObject(JsonSerializer&& value)
			{
				m_value = std::move(value.m_value);
				return *this;
			}

			JsonSerializer JsonSerializer::AsObject() const
			{
				return m_value;
			}

			Nequeo::Map<Nequeo::String, JsonSerializer> JsonSerializer::GetAllObjects() const
			{
				Nequeo::Map<Nequeo::String, JsonSerializer> valueMap;

				for (Json::ValueIterator iter = m_value.begin(); iter != m_value.end(); ++iter)
				{
					Nequeo::String strString(iter.key().asString().c_str());
					valueMap[strString] = *iter;
				}

				return valueMap;
			}

			bool JsonSerializer::ValueExists(const char* key) const
			{
				return m_value.isMember(key);
			}

			bool JsonSerializer::ValueExists(const Nequeo::String& key) const
			{
				return ValueExists(key.c_str());
			}

			bool JsonSerializer::IsObject() const
			{
				return m_value.isObject();
			}

			bool JsonSerializer::IsBool() const
			{
				return m_value.isBool();
			}

			bool JsonSerializer::IsString() const
			{
				return m_value.isString();
			}

			bool JsonSerializer::IsIntegerType() const
			{
				return m_value.isIntegral();
			}

			bool JsonSerializer::IsFloatingPointType() const
			{
				return m_value.isDouble();
			}

			bool JsonSerializer::IsListType() const
			{
				return m_value.isArray();
			}

			std::string JsonSerializer::WriteCompact(bool treatAsObject) const
			{
				if (treatAsObject && m_value.isNull())
				{
					return "{}";
				}

				Json::FastWriter fastWriter;
				return fastWriter.write(m_value);
			}

			void JsonSerializer::WriteCompact(Nequeo::OStream& ostream, bool treatAsObject) const
			{
				if (treatAsObject && m_value.isNull())
				{
					ostream << "{}";
					return;
				}

				std::string compactString = WriteCompact();
				ostream.write(compactString.c_str(), compactString.length());
			}

			std::string JsonSerializer::WriteReadable(bool treatAsObject) const
			{
				if (treatAsObject && m_value.isNull())
				{
					return "{\n}\n";
				}

				Json::StyledWriter styledWriter;
				return styledWriter.write(m_value);
			}

			void JsonSerializer::WriteReadable(Nequeo::OStream& ostream, bool treatAsObject) const
			{
				if (treatAsObject && m_value.isNull())
				{
					ostream << "{\n}\n";
				}

				Json::StyledStreamWriter styledStreamWriter;
				styledStreamWriter.write(ostream, m_value);
			}
		}
	}
}