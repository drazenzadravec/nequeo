#pragma once

#ifndef _MANAGEDSTATICPROXY_H
#define _MANAGEDSTATICPROXY_H

#include "stdafx.h"

#include "ManagedProxyBase.h"

#include "tchar.h"
#include <string>

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

ref class ManagedStaticProxy : public ManagedProxyBase
{
public:
    ManagedStaticProxy(const stdtstring &dllName, const stdtstring &managedTypeName);

protected:
    virtual System::Type^ getManagedType() override;
    virtual System::Object^ invokeFunction(System::String^ functionName, cli::array<System::Object^>^ params, System::Reflection::BindingFlags bindingFlags) override;

private:
    System::Type^ managedType_;
};

#endif
