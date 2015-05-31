//*****************************************************************************
//
//  Author:         Drazen Zadravec
//  Date created:   Copyright 2012
//
//  Description:    The internal Strings class is used to load strings from the
//                  embedded string table.
//
//*****************************************************************************

#pragma once

namespace NequeoSecurity
{
    ref class Strings abstract sealed
    {
		public:

			static String^ Get(String^ key);

		private:

			static Strings();
			static initonly Resources::ResourceManager^ m_resources;

    };
}
