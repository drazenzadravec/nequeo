#include "stdafx.h"

#include "ManagedProxyBase.h"
#include "Vcclr.h"

using namespace System;
using namespace System::IO;
using namespace System::Reflection;

ManagedProxyBase::~ManagedProxyBase(void)
{
}

String^ ManagedProxyBase::locateAssembly(String^ dllFilename)
{
    String^ BaseDirectory = gcnew String(L"NequeoManagedModules");
    FileInfo^ fi = gcnew FileInfo(dllFilename);

	if (!fi->Exists)
	{
        dllFilename = Path::Combine(gcnew String(BaseDirectory), fi->Name);
		if (!File::Exists(dllFilename))
		{
            FileInfo^ fib = gcnew FileInfo(Assembly::GetExecutingAssembly()->Location);

			dllFilename = Path::Combine(fib->DirectoryName,fi->Name);
			if (!File::Exists(dllFilename))
                throw gcnew DllNotFoundException(dllFilename);
		}
	}
    return dllFilename;
}

cli::array<Object^>^ ManagedProxyBase::cliArrayFromAnyArray(const AnyTypeArray &anyArray)
{
    int len = static_cast<int>(anyArray.size());
    cli::array<Object^>^ cliArray = gcnew cli::array<Object^>(len);
    for(int i = 0; i < len; ++i)
        cliArray[i] = toObject(anyArray[i]);
    return cliArray;
}

void ManagedProxyBase::retrieveOutputParameters(MethodInfo^ functionInfo, cli::array<Object^> ^params, AnyTypeArray &inputParams)
{    
    cli::array<ParameterInfo^>^paramInfos = functionInfo->GetParameters();
    Collections::IEnumerator^ paramPos = paramInfos->GetEnumerator();
    while ( paramPos->MoveNext() )
    {
        ParameterInfo^ currentParam = safe_cast<ParameterInfo^>(paramPos->Current);
        if ((currentParam->IsOut || currentParam->ParameterType->IsByRef) && !currentParam->IsRetval)
        {
            toAny(params[currentParam->Position], inputParams[currentParam->Position]);
        }
    }
}

Type^ ManagedProxyBase::getTypeFromFile(String^ dllFilename, String^ typeName)
{
    Assembly^ dllAssembly = nullptr;

    if (!dllFilename->EndsWith(".dll", System::StringComparison::InvariantCultureIgnoreCase))
        dllAssembly = Assembly::Load(dllFilename);
    else
        dllAssembly = Assembly::LoadFrom(locateAssembly(dllFilename));

    return dllAssembly->GetType(typeName);
}

void ManagedProxyBase::systemStringToTString(System::String^ source, stdtstring &destination)
{
    pin_ptr<const wchar_t> pinString = PtrToStringChars(source);
    std::wstring wsource(pinString);
#ifdef _UNICODE
	destination = wsource;
#else
    destination = stringtools::toAsciiString(wsource);
#endif 
}


void ManagedProxyBase::call(const stdtstring &function, AnyType &result)
{
    AnyTypeArray noParameters;
    call(function, noParameters, result);
}

void ManagedProxyBase::call(const stdtstring &function, AnyTypeArray &inOutParameters, AnyType &result)
{
    cli::array<Object^>^ params = cliArrayFromAnyArray(inOutParameters);

    String^ functionName = gcnew String(function.c_str());
    Object^ returnValue = invokeFunction(functionName, params, BindingFlags::Public | BindingFlags::InvokeMethod );

    // copy out- or ref-parameters back
    retrieveOutputParameters(getManagedType()->GetMethod(functionName), params, inOutParameters);
    // get result value here 
    toAny(returnValue, result);
}

void ManagedProxyBase::setProperty(const stdtstring &propertyName, const AnyType &value)
{
    cli::array<Object^>^ params = gcnew cli::array<Object^>(1);
    params[0] = toObject(value);

    invokeFunction(gcnew String(propertyName.c_str()), params, BindingFlags::Public | BindingFlags::SetProperty);    
}

void ManagedProxyBase::getProperty(const stdtstring &propertyName, AnyType &value)
{
    Object^ returnValue = invokeFunction(gcnew String(propertyName.c_str()), nullptr, BindingFlags::Public | BindingFlags::GetProperty);    
    // get result value here 
    toAny(returnValue, value);
}
