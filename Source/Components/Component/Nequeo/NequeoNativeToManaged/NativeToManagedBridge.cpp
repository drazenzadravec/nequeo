// NativeToManagedBridge.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "Base\NativeHandler.h"
#include "ManagedProxy.h"
#include "ManagedStaticProxy.h"

#ifdef _UNICODE
#define CreateNetObject CreateNetObjectW
#define CallNetObject CallNetObjectW
#define GetNetObjectProperty GetNetObjectPropertyW
#define SetNetObjectProperty SetNetObjectPropertyW
#define CallNetClass CallNetClassW
#define GetNetClassProperty GetNetClassPropertyW
#define SetNetClassProperty SetNetClassPropertyW
#define ReleaseNetObject ReleaseNetObjectW
#else
#define CreateNetObject CreateNetObjectA
#define CallNetObject CallNetObjectA
#define GetNetObjectProperty GetNetObjectPropertyA
#define SetNetObjectProperty SetNetObjectPropertyA
#define CallNetClass CallNetClassA
#define GetNetClassProperty GetNetClassPropertyA
#define SetNetClassProperty SetNetClassPropertyA
#define ReleaseNetObject ReleaseNetObjectA
#endif 

extern "C"
{
    __declspec(dllexport) void * CreateNetObject(const stdtstring &dllFileName, const stdtstring &className, const AnyTypeArray &ctorParams, stdtstring &exceptionText)
    {
	    try
	    {
            ManagedProxy^ proxy = ManagedProxy::create(dllFileName, className, ctorParams);
            return proxy->nativePointer();
	    }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return 0;
	    }
    }

    __declspec(dllexport) bool ReleaseNetObject(void *pointerToManagedObject, stdtstring &exceptionText)
    {
        try
        {
            ManagedProxy^ proxy = gcnew ManagedProxy(pointerToManagedObject);
            proxy->release();
            return true;
        }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }

    // instance 
    __declspec(dllexport) bool CallNetObject(void *pointerToManagedObject, const stdtstring &functionName, AnyTypeArray &inputParameters, AnyType &result, stdtstring &exceptionText)
	{
	    try
	    {
            ManagedProxy^ proxy = gcnew ManagedProxy(pointerToManagedObject);
            proxy->call(functionName, inputParameters, result);
            return true;
	    }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }

    __declspec(dllexport) bool GetNetObjectProperty(void *pointerToManagedObject, const stdtstring &propertyName, AnyType &result, stdtstring &exceptionText)
    {
        try
        {
            ManagedProxy^ proxy = gcnew ManagedProxy(pointerToManagedObject);
            proxy->getProperty(propertyName, result);
            return true;
        }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }

    __declspec(dllexport) bool SetNetObjectProperty(void *pointerToManagedObject, const stdtstring &propertyName, const AnyType &value, stdtstring &exceptionText)
    {
        try
        {
            ManagedProxy^ proxy = gcnew ManagedProxy(pointerToManagedObject);
            proxy->setProperty(propertyName, value);
            return true;
        }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }
    // statics
    __declspec(dllexport) bool CallNetClass(const stdtstring &dllFileName, const stdtstring &className, const stdtstring &functionName, AnyTypeArray &inputParameters, AnyType &result, stdtstring &exceptionText)
	{
	    try
	    {
            ManagedStaticProxy^ proxy = gcnew ManagedStaticProxy(dllFileName, className);
            proxy->call(functionName, inputParameters, result);
            return true;
	    }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }

    __declspec(dllexport) bool GetNetClassProperty(const stdtstring &dllFileName, const stdtstring &className, const stdtstring &propertyName, AnyType &result, stdtstring &exceptionText)
    {
        try
        {
            ManagedStaticProxy^ proxy = gcnew ManagedStaticProxy(dllFileName, className);
            proxy->getProperty(propertyName, result);
            return true;
        }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }

    __declspec(dllexport) bool SetNetClassProperty(const stdtstring &dllFileName, const stdtstring &className, const stdtstring &propertyName, const AnyType &value, stdtstring &exceptionText)
    {
        try
        {
            ManagedStaticProxy^ proxy = gcnew ManagedStaticProxy(dllFileName, className);
            proxy->setProperty(propertyName, value);
            return true;
        }
        catch(System::Exception ^ex)
	    {
            ManagedProxyBase::systemStringToTString(ex->ToString(), exceptionText);    
            return false;
	    }
    }
}
