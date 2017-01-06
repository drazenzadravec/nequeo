#pragma once

#include "GlobalBase.h"

// This is an example of a class exported from the NequeoBase.dll
class NequeoBase_API CNequeoBase
{
public:
    CNequeoBase();
    // TODO: add your methods here.
};

// This is an example of an exported variable
extern NequeoBase_API int nNequeoBase;

// This is an example of an exported function.
NequeoBase_API int fnNequeoBase(void);
