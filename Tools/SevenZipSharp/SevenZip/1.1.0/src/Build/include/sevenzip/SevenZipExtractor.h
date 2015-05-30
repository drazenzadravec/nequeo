#pragma once


#include "SevenZipLibrary.h"
#include "CompressionFormat.h"

namespace SevenZip
{
	class SevenZipExtractor
	{
	private:

		const SevenZipLibrary& m_library;
		TString m_archivePath;
		CompressionFormatEnum m_format;
		const wchar_t *m_password;
		bool m_passwordSet;

	public:

		SevenZipExtractor( const SevenZipLibrary& library, const TString& archivePath );
		virtual ~SevenZipExtractor();

		void SetCompressionFormat( const CompressionFormatEnum& format );
		void SetPassword(const wchar_t *password);

		virtual void ExtractArchive( const TString& directory );

	private:

		void ExtractArchive( const CComPtr< IStream >& archiveStream, const TString& directory );
	};
}
