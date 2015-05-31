//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    The Credential class models a single user credential.
//
//*****************************************************************************

#pragma once

#include "CredentialType.h"
#include "CredentialPersistence.h"

using namespace System;

namespace NequeoSecurity {

	// Credential class
	public ref class Credential
	{
		public:
			// Constructs.
			Credential(Credential^ copy);

			Credential(String^ targetName, CredentialType type);

			Credential(String^ targetName, CredentialType type, String^ userName,
                   Security::SecureString^ password, CredentialPersistence persistence,
                   String^ description);
        
			~Credential();

			// Properties
			property String^ TargetName
			{
				String^ get();
			}

			property CredentialType Type
			{
				CredentialType get();
			}

			property String^ UserName
			{
				String^ get();
				void set(String^ value);
			}

			property Security::SecureString^ Password
			{
				Security::SecureString^ get();
				void set(Security::SecureString^ value);
			}

			property CredentialPersistence Persistence
			{
				CredentialPersistence get();
				void set(CredentialPersistence value);
			}

			property String^ Description
			{
				String^ get();
				void set(String^ value);
			}

			property DateTime LastWriteTime
			{
				DateTime get();
			}

			property DateTime LastWriteTimeUtc
			{
				DateTime get();
			}

			property int SecrectLength
			{
				int get();
			}

			// Methods
			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			void Load();
        
			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			void Save();
        
			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			void Delete();

			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			void ChangeTargetName(String^ newTargetName);

			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			static bool Exists(String^ targetName, CredentialType type);

			[SecurityPermission(SecurityAction::LinkDemand, Flags=SecurityPermissionFlag::UnmanagedCode)]
			static void Delete(String^ targetName, CredentialType type);


		internal:
			// Constructs.
			Credential(PCREDENTIAL credential);


		private:
			void Initialize(PCREDENTIAL credential);

			void Initialize(String^ targetName,
							CredentialType type,
							String^ userName,
							Security::SecureString^ password,
							int secretLength,
							CredentialPersistence persistence,
							String^ description,
							DateTime lastWritten);

			void CheckNotDisposed();

			bool m_disposed;

			String^ m_targetName;
			CredentialType m_type;

			String^ m_userName;
			Security::SecureString^ m_password;
			int m_secretLength;

			CredentialPersistence m_persistence;
			String^ m_description;
			DateTime m_lastWritten;
	};
}
