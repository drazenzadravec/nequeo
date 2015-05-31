//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    Specifies the persistence of a credential.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
	// Credential Persistence
    public enum class CredentialPersistence
    {
        None = 0,
        Session = CRED_PERSIST_SESSION,
        LocalComputer = CRED_PERSIST_LOCAL_MACHINE,
        Enterprise = CRED_PERSIST_ENTERPRISE
    };
}