//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    CredentialNotFoundException is the exception that is thrown
//                  when an attempt to load or reference a credential that does
//                  not exist fails.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
    [Serializable]
    public ref class CredentialNotFoundException : Exception
    {
		internal:

			CredentialNotFoundException(ComponentModel::Win32Exception^ innerException);

		protected:

			CredentialNotFoundException(Runtime::Serialization::SerializationInfo^ info,
										Runtime::Serialization::StreamingContext context);
    };
}
