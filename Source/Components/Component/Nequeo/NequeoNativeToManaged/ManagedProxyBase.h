#pragma once

#ifndef _MANAGEDPROXYBASE_H
#define _MANAGEDPROXYBASE_H

#include "stdafx.h"

#include "anyHelper.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

ref class ManagedProxyBase abstract
{
public:
    virtual ~ManagedProxyBase(void);

    static void systemStringToTString(System::String^ source, stdtstring &destination);
    void call(const stdtstring &function, AnyType &result);
    void call(const stdtstring &function, AnyTypeArray &inOutParameters, AnyType &result);
    void setProperty(const stdtstring &propertyName, const AnyType &value);
    void getProperty(const stdtstring &propertyName, AnyType &value);

protected:
    static System::String^ locateAssembly(System::String^ dllFilename);
    static cli::array<System::Object^>^ cliArrayFromAnyArray(const AnyTypeArray &anyArray);
    static void retrieveOutputParameters(System::Reflection::MethodInfo^ functionInfo, cli::array<System::Object^> ^params, AnyTypeArray &inputParams);
    static System::Type^ getTypeFromFile(System::String^ dllFilename, System::String^ typeName);

    virtual System::Object^ invokeFunction(System::String^ functionName, cli::array<System::Object^>^ params, System::Reflection::BindingFlags bindingFlags) = 0;
    virtual System::Type^ getManagedType() = 0;
};

#endif 
