#include "stdafx.h"

#include "ManagedProxy.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

ManagedProxy::ManagedProxy(void *ptr2ManagedObject)
    : ptr2ManagedObject_(ptr2ManagedObject)
{
}

ManagedProxy::ManagedProxy(Object^ instance)
    : ptr2ManagedObject_(nativePointerFromManagedObject(instance))
{
}

ManagedProxy^ ManagedProxy::create(const stdtstring &dllName, const stdtstring &managedTypeName, const AnyTypeArray &ctorParams)
{
    // copy parameters in here 
    cli::array<Object^>^ params = cliArrayFromAnyArray(ctorParams);

    Type ^type = getTypeFromFile(gcnew String(dllName.c_str()), gcnew String(managedTypeName.c_str()));
    Object ^instance = type->InvokeMember(nullptr
                                          , BindingFlags::CreateInstance
                                          , nullptr
                                          , nullptr
                                          , params);
    return gcnew ManagedProxy(instance);        
}

void ManagedProxy::release()
{
    if (ptr2ManagedObject_)
    {            
        GCHandle objectHandle = GCHandle::FromIntPtr(IntPtr(ptr2ManagedObject_));
        objectHandle.Free();
        ptr2ManagedObject_ = 0;
    }
}

void *ManagedProxy::nativePointer()
{
    return ptr2ManagedObject_;
}

Object^ ManagedProxy::invokeFunction(String^ functionName, cli::array<Object^>^ params, BindingFlags bindingFlags)
{
    return instance()->GetType()->InvokeMember(functionName
                                                , bindingFlags | BindingFlags::Instance | BindingFlags::Static // allow static calls through instance 
                                                , nullptr   // default binder 
                                                , instance()
                                                , params);
}

Type^ ManagedProxy::getManagedType()
{
    return instance()->GetType();
}

Object^ ManagedProxy::instance()
{
    return GCHandle::FromIntPtr(IntPtr(ptr2ManagedObject_)).Target;
}

void *ManagedProxy::nativePointerFromManagedObject(Object^ instance) 
{
    GCHandle objectHandle = GCHandle::Alloc(instance);
    return objectHandle.ToIntPtr(objectHandle).ToPointer();
}

