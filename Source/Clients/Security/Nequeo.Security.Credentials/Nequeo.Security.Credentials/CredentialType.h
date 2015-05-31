//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    Specifies the type of a credential. The type cannot be 
//                  changed after a particular credential is created.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
	// Credential Type
    public enum class CredentialType
    {
        None = 0,
        Generic = CRED_TYPE_GENERIC,
        DomainPassword = CRED_TYPE_DOMAIN_PASSWORD,
        DomainCertificate = CRED_TYPE_DOMAIN_CERTIFICATE,
        DomainVisiblePassword = CRED_TYPE_DOMAIN_VISIBLE_PASSWORD
    };
}