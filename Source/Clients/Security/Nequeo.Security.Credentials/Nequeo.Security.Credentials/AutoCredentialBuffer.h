//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    The AutoCredentialBuffer template class takes care of 
//                  automatically freeing buffers returned by the various
//                  credentials management functions.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
    template <typename T>
    class AutoCredentialBuffer
    {
		public:

			AutoCredentialBuffer() :
				m_p(0)
			{
            // Do nothing
			}
			~AutoCredentialBuffer()
			{
				if (0 != m_p)
				{
					::CredFree(m_p);
				}
			}

			T m_p;

		private:

			AutoCredentialBuffer(const AutoCredentialBuffer&);
			AutoCredentialBuffer& operator=(const AutoCredentialBuffer&);
    };
}