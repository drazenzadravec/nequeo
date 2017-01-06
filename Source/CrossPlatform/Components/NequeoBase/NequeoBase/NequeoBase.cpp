// NequeoBase.cpp : Defines the exported functions for the DLL application.
//

#include "NequeoBase.h"

// This is an example of an exported variable
NequeoBase_API int nNequeoBase=0;

// This is an example of an exported function.
NequeoBase_API int fnNequeoBase(void)
{
    return 42;
}

// This is the constructor of a class that has been exported.
// see NequeoBase.h for the class definition
CNequeoBase::CNequeoBase()
{
    return;
}
