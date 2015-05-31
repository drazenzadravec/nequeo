//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    DuplicateTargetNameException is the exception that is 
//                  thrown when an attempt is made to change a credential's
//                  target name but a different credential already occupies 
//                  that name.
//
//*****************************************************************************

#include "stdafx.h"
#include "DuplicateTargetNameException.h"
#include "Strings.h"

using namespace Runtime::Serialization;

NequeoSecurity::DuplicateTargetNameException::DuplicateTargetNameException(ComponentModel::Win32Exception^ innerException) :
    Exception(Strings::Get("DuplicateTargetNameException.Message"), innerException)
{
    // Do nothing
}

NequeoSecurity::DuplicateTargetNameException::DuplicateTargetNameException(SerializationInfo^ info,
                                                                 StreamingContext context) :
    Exception(info, context)
{
    // Do nothing
}
