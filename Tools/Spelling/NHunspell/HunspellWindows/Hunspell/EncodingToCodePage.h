
inline UINT EncodingToCodePage( char  * encoding )
{
	if( stricmp(encoding,"utf8") == 0 || stricmp(encoding,"utf-8") == 0 )
		return CP_UTF8;

	else if( stricmp(encoding,"iso-8859-1") == 0 || stricmp(encoding,"iso8859-1") == 0 )
		return 28591;

	else if( stricmp(encoding,"iso-8859-2") == 0 || stricmp(encoding,"iso8859-2") == 0 )
		return 28592;

	else if( stricmp(encoding,"iso-8859-3") == 0 || stricmp(encoding,"iso8859-3") == 0 )
		return 28593;

	else if( stricmp(encoding,"iso-8859-4") == 0 || stricmp(encoding,"iso8859-4") == 0 )
		return 28594;

	else if( stricmp(encoding,"iso-8859-5") == 0 || stricmp(encoding,"iso8859-5") == 0 )
		return 28595;

	else if( stricmp(encoding,"iso-8859-6") == 0 || stricmp(encoding,"iso8859-6") == 0 )
		return 28596;

	else if( stricmp(encoding,"iso-8859-7") == 0 || stricmp(encoding,"iso8859-7") == 0 )
		return 28597;

	else if( stricmp(encoding,"iso-8859-8") == 0 || stricmp(encoding,"iso8859-8") == 0 )
		return 28598;

	else if( stricmp(encoding,"iso-8859-9") == 0 || stricmp(encoding,"iso8859-9") == 0 )
		return 28599;

	else if( stricmp(encoding,"iso-8859-13") == 0 || stricmp(encoding,"iso8859-13") == 0 )
		return 28603;

	else if( stricmp(encoding,"iso-8859-15") == 0 || stricmp(encoding,"iso8859-15") == 0 )
		return 28605;

	else if( stricmp(encoding,"windows-1250") == 0 || stricmp(encoding,"microsoft-cp1250") == 0 )
		return 1250;

	else if( stricmp(encoding,"windows-1251") == 0 || stricmp(encoding,"microsoft-cp1251") == 0 )
		return 1251;

	else if( stricmp(encoding,"windows-1252") == 0 || stricmp(encoding,"microsoft-cp1252") == 0 )
		return 1252;

	else if( stricmp(encoding,"windows-1253") == 0 || stricmp(encoding,"microsoft-cp1253") == 0 )
		return 1253;

	else if( stricmp(encoding,"windows-1254") == 0 || stricmp(encoding,"microsoft-cp1254") == 0 )
		return 1254;

	else if( stricmp(encoding,"windows-1255") == 0 || stricmp(encoding,"microsoft-cp1255") == 0 )
		return 1255;

	else if( stricmp(encoding,"windows-1256") == 0 || stricmp(encoding,"microsoft-cp1256") == 0 )
		return 1256;

	else if( stricmp(encoding,"windows-1257") == 0 || stricmp(encoding,"microsoft-cp1257") == 0 )
		return 1257;

	else if( stricmp(encoding,"windows-1258") == 0 || stricmp(encoding,"microsoft-cp1258") == 0 )
		return 1258;

	else if( stricmp(encoding,"windows-1259") == 0 || stricmp(encoding,"microsoft-cp1259") == 0 )
		return 1259;

	else if( stricmp(encoding,"koi8-r") == 0 || stricmp(encoding,"koi8-u") == 0 )
		return 20866;

	else
		return 1252;
}
