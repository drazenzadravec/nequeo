//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    UserNameInvalidException is the exception that is thrown 
//                  when an invalid user name is specified.
//
//*****************************************************************************

#include "stdafx.h"
#include "UserNameInvalidException.h"
#include "Strings.h"

using namespace Runtime::Serialization;

NequeoSecurity::UserNameInvalidException::UserNameInvalidException(ComponentModel::Win32Exception^ innerException) :
    Exception(Strings::Get("UserNameInvalidException.Message"), innerException)
{
    // Do nothing
}

NequeoSecurity::UserNameInvalidException::UserNameInvalidException(SerializationInfo^ info,
                                                         StreamingContext context) :
    Exception(info, context)
{
    // Do nothing
}
