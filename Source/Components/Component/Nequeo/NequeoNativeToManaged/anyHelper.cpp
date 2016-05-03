#include "stdafx.h"

#include "anyHelper.h"
#include "Vcclr.h"

namespace 
{
    System::String^ anyAsciiStringToSystemString(const AnyAsciiString &source);
    System::String^ anyUnicodeStringToSystemString(const AnyUnicodeString &source);
    System::DateTime^ anyDateTimeToDateTime(const AnyDateTime &datetime);
    cli::array<System::Byte>^ anyByteArrayToByteArray(const AnyByteArray &sourceData);
	cli::array<System::String^>^ anyAsciiStringArrayToStringArray(const AnyAsciiStringArray &source);
	cli::array<System::String^>^ anyUnicodeStringArrayToStringArray(const AnyUnicodeStringArray &source);
    const AnyUnicodeString systemStringToAnyUnicodeString(System::String^ source);

    void toAny(System::DateTime^ sourceDateTime, AnyType &result);
    void toAny(System::String^ sourceString, AnyType &result);
    void toAny(cli::array<System::Byte>^ sourceData, AnyType &result);
	void toAny(cli::array<System::String^>^ sourceData, AnyType &result);
}

System::Object^ toObject(const AnyType &source)
{    
    if (source.type() == typeid(AnyBoolean))
        return boost::any_cast<AnyBoolean>(source);    
    else if (source.type() == typeid(AnyInt64))
        return boost::any_cast<AnyInt64>(source);
    else if (source.type() == typeid(AnyInt32))
        return boost::any_cast<AnyInt32>(source);
    else if (source.type() == typeid(AnyInt16))
        return boost::any_cast<AnyInt16>(source);
    else if (source.type() == typeid(AnyInt8))
        return boost::any_cast<AnyInt8>(source);
    else if (source.type() == typeid(AnyUInt64))
        return boost::any_cast<AnyUInt64>(source);
    else if (source.type() == typeid(AnyUInt32))
        return boost::any_cast<AnyUInt32>(source);
    else if (source.type() == typeid(AnyUInt16))
        return boost::any_cast<AnyUInt16>(source);
    else if (source.type() == typeid(AnyUInt8))
        return boost::any_cast<AnyUInt8>(source);
    else if (source.type() == typeid(AnyFloat128))
        return boost::any_cast<AnyFloat128>(source);
    else if (source.type() == typeid(AnyFloat64))
        return boost::any_cast<AnyFloat64>(source);
    else if (source.type() == typeid(AnyFloat32))
        return boost::any_cast<AnyFloat32>(source);
    else if (source.type() == typeid(AnyDateTime))
        return anyDateTimeToDateTime(boost::any_cast<AnyDateTime>(source));
    else if (source.type() == typeid(AnyAsciiChar))
        return boost::any_cast<AnyAsciiChar>(source);
    else if (source.type() == typeid(AnyUnicodeChar))
        return boost::any_cast<AnyUnicodeChar>(source);
    else if (source.type() == typeid(AnyAsciiString))
        return anyAsciiStringToSystemString(boost::any_cast<AnyAsciiString>(source));
    else if (source.type() == typeid(AnyUnicodeString))
        return anyUnicodeStringToSystemString(boost::any_cast<AnyUnicodeString>(source));
    else if (source.type() == typeid(AnyByteArray))
        return anyByteArrayToByteArray(boost::any_cast<AnyByteArray>(source));
	else if (source.type() == typeid(AnyAsciiStringArray))
		return anyAsciiStringArrayToStringArray(boost::any_cast<AnyAsciiStringArray>(source));
	else if (source.type() == typeid(AnyUnicodeStringArray))
		return anyUnicodeStringArrayToStringArray(boost::any_cast<AnyUnicodeStringArray>(source));

	return nullptr;
}

void toAny(System::Object ^source, AnyType &destination)
{    
	if (source == nullptr)
	{
		destination = AnyType();
		return;
	}

    System::Type^ type = source->GetType();

    if (type == System::Boolean::typeid)
    {
        AnyBoolean value = safe_cast<System::Boolean>(source);
        destination = value;
    }
    else if (type == System::SByte::typeid)
    {
        AnyInt8 value = safe_cast<System::SByte>(source);
        destination = value;
    }
    else if (type == System::Int16::typeid)
    {
        AnyInt16 value = safe_cast<System::Int16>(source);
        destination = value;
    }
    else if (type == System::Int32::typeid)
    {
        AnyInt32 value = safe_cast<System::Int32>(source);
        destination = value;
    }
    else if (type == System::Int64::typeid)
    {
        AnyInt64 value = safe_cast<System::Int64>(source);
        destination = value;
    }
    else if (type == System::Byte::typeid)
    {
        AnyUInt8 value = safe_cast<System::Byte>(source);
        destination = value;
    }
    else if (type == System::UInt16::typeid)
    {
        AnyUInt16 value = safe_cast<System::UInt16>(source);
        destination = value;
    }
    else if (type == System::UInt32::typeid)
    {
        AnyUInt32 value = safe_cast<System::UInt32>(source);
        destination = value;
    }
    else if (type == System::UInt64::typeid)
    {
        AnyUInt64 value = safe_cast<System::UInt64>(source);
        destination = value;
    }
    else if (type == System::Single::typeid)
    {
        AnyFloat32 value = safe_cast<System::Single>(source);
        destination = value;
    }
    else if (type == System::Double::typeid)
    {
        AnyFloat64 value = safe_cast<System::Double>(source);
        destination = value;
    }
    else if (type == System::DateTime::typeid)
    {
        toAny(safe_cast<System::DateTime^>(source), destination);
    }
    else if (type == System::Char::typeid)
    {
        AnyUnicodeChar value = safe_cast<System::Char>(source);
        destination = value;
    }
    else if (type == System::String::typeid)
    {
        toAny(safe_cast<System::String^>(source), destination);
    }
    else if (type == cli::array<System::Byte>::typeid)
    {
        toAny(safe_cast<cli::array<System::Byte>^>(source), destination);
    }
	else if (type == cli::array<System::String>::typeid)
	{
		toAny(safe_cast<cli::array<System::String^>^>(source), destination);
	}
}

namespace
{

System::DateTime^ anyDateTimeToDateTime(const AnyDateTime &datetime)
{
    return gcnew System::DateTime(datetime.tm_year + 1900
                                    , datetime.tm_mon + 1
                                    , datetime.tm_mday
                                    , datetime.tm_hour
                                    , datetime.tm_min
                                    , datetime.tm_sec);
}

cli::array<System::Byte>^ anyByteArrayToByteArray(const AnyByteArray &sourceData)
{
    int len = static_cast<int>(sourceData.size());
    cli::array<System::Byte>^ data = gcnew cli::array<System::Byte>(len);
    for (int i = 0; i < len; i++)
        data[i] = sourceData[i];
    return data;
}

cli::array<System::String^>^ anyAsciiStringArrayToStringArray(const AnyAsciiStringArray &source)
{
	int len = static_cast<int>(source.size());
	cli::array<System::String^>^ data = gcnew cli::array<System::String^>(len);
	for (int i = 0; i < len; i++)
		data[i] = anyAsciiStringToSystemString(source[i]);
	return data;
}

cli::array<System::String^>^ anyUnicodeStringArrayToStringArray(const AnyUnicodeStringArray &source)
{
	int len = static_cast<int>(source.size());
	cli::array<System::String^>^ data = gcnew cli::array<System::String^>(len);
	for (int i = 0; i < len; i++)
		data[i] = anyUnicodeStringToSystemString(source[i]);
	return data;
}

System::String^ anyAsciiStringToSystemString(const AnyAsciiString &source)
{
    return gcnew System::String(source.c_str());
}

System::String^ anyUnicodeStringToSystemString(const AnyUnicodeString &source)
{
    return gcnew System::String(source.c_str());
}

void toAny(System::DateTime^ sourceDateTime, AnyType &result)
{
    AnyDateTime datetime;
    datetime.tm_year = sourceDateTime->Year - 1900;
    datetime.tm_mon = sourceDateTime->Month - 1;
    datetime.tm_mday = sourceDateTime->Day;
    datetime.tm_hour = sourceDateTime->Hour;
    datetime.tm_min = sourceDateTime->Minute;
    datetime.tm_sec = sourceDateTime->Second;
    datetime.tm_isdst = sourceDateTime->IsDaylightSavingTime();
    datetime.tm_wday = (int)sourceDateTime->DayOfWeek;
    datetime.tm_yday = sourceDateTime->DayOfYear;
    result = datetime;
}

void toAny(System::String^ sourceString, AnyType &result)
{
    pin_ptr<const wchar_t> pinString = PtrToStringChars(sourceString);
    std::wstring value(pinString);
    result = value;
}

void toAny(cli::array<System::Byte>^ sourceData, AnyType &result)
{
    AnyByteArray data(sourceData->Length);
    for (int i = 0; i < static_cast<int>(data.size()); i++)
        data[i] = sourceData[i];
    result = data;
}

void toAny(cli::array<System::String^>^ sourceData, AnyType &result)
{
	AnyUnicodeStringArray data(sourceData->Length);
	for (int i = 0; i < static_cast<int>(data.size()); i++)
		data[i] = systemStringToAnyUnicodeString(sourceData[i]);
	result = data;
}

const AnyUnicodeString systemStringToAnyUnicodeString(System::String^ source)
{
    pin_ptr<const wchar_t> pinString = PtrToStringChars(source);
    return AnyUnicodeString(pinString);
}

}
