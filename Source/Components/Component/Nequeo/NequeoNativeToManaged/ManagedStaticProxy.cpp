#include "StdAfx.h"
#include "ManagedStaticProxy.h"

using namespace System;
using namespace System::Reflection;

ManagedStaticProxy::ManagedStaticProxy(const stdtstring &dllName, const stdtstring &managedTypeName)
    : managedType_(getTypeFromFile(gcnew String(dllName.c_str()), gcnew String(managedTypeName.c_str())))
{
}

Object^ ManagedStaticProxy::invokeFunction(String^ functionName, cli::array<Object^>^ params, BindingFlags bindingFlags)
{
    return managedType_->InvokeMember(functionName
                                        , bindingFlags | BindingFlags::Static
                                        , nullptr   // default binder 
                                        , nullptr   // no instance 
                                        , params);
}

System::Type^ ManagedStaticProxy::getManagedType()
{
    return managedType_;
}
