#pragma once

#ifndef _ANYHELPER_H
#define _ANYHELPER_H

#include "stdafx.h"

#include "Base\Types.h"

using namespace Nequeo::System::Any;

System::Object^ toObject(const AnyType &source);
void toAny(System::Object ^source, AnyType &destination);

#endif
