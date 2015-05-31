//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    UserNameInvalidException is the exception that is thrown 
//                  when an invalid user name is specified.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
    [Serializable]
    public ref class UserNameInvalidException : Exception
    {
		internal:

			UserNameInvalidException(ComponentModel::Win32Exception^ innerException);

		protected:

			UserNameInvalidException(Runtime::Serialization::SerializationInfo^ info,
									 Runtime::Serialization::StreamingContext context);
    };
}
