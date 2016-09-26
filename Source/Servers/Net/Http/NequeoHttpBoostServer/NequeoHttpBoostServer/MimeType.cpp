/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MimeType.cpp
*  Purpose :       Mime types.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "stdafx.h"

#include "MimeType.h"
#include "Base\StringUtils.h"

using namespace Nequeo::Net::Mime;

///	<summary>
///	Mime types.
///	</summary>
MimeType::MimeType() :
	_disposed(false)
{
	CreateApplicationTypes();
	CreateMimeTypes();
}

///	<summary>
///	Mime types.
///	</summary>
MimeType::~MimeType()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Is the mime type an application hosting type.
/// </summary>
/// <param name="extension">The extension.</param>
/// <returns>True if application hosting type; else false.</returns>
bool MimeType::IsApplicationType(const std::string& extension)
{
	// Remove the dot a begining if exists.
	std::string extNoDot = extension;

	// Find the index.
	std::basic_string <char>::size_type index;
	index = extension.find(".");

	// If a dot has been found.
	if (index != std::string::npos)
	{
		// If the dot is at position 1.
		if (index == 1)
		{
			// Ext without the dot.
			// Move to the second char, this is index 0 based.
			extNoDot = extension.substr(1);
		}
	}

	// Make to lower.
	Nequeo::String toLowerNoDot = Nequeo::StringUtils::ToLower(extNoDot.c_str());
	std::string ext(toLowerNoDot.c_str());

	// If exists.
	if (std::find(_applicationTypes.begin(), _applicationTypes.end(), ext) != _applicationTypes.end())
	{
		return true;
	}
	else
	{
		return false;
	}
}

/// <summary>
/// Get the mime type for the extension.
/// </summary>
/// <param name="extension">The extension.</param>
/// <returns>The mime type.</returns>
std::string MimeType::GetMimeType(const std::string& extension)
{
	// Remove the dot a begining if exists.
	std::string extNoDot = extension;

	// Find the index.
	std::basic_string <char>::size_type index;
	index = extension.find(".");

	// If a dot has been found.
	if (index != std::string::npos)
	{
		// If the dot is at position 1.
		if (index == 0)
		{
			// Ext without the dot.
			// Move to the second char, this is index 0 based.
			extNoDot = extension.substr(1);
		}
	}

	// Make to lower.
	Nequeo::String toLowerNoDot = Nequeo::StringUtils::ToLower(extNoDot.c_str());
	std::string ext(toLowerNoDot.c_str());

	// Find existing mime.
	auto mime_it = _mimeTypes.find(ext);

	// If found mime.
	if (mime_it != _mimeTypes.end())
	{
		// Get the mime for the extension.
		auto mime_value = mime_it->second;
		return mime_value;
	}
	else
	{
		// Return the default.
		return "application/plain";
	}
}

/// <summary>
/// Create the application types.
/// </summary>
void MimeType::CreateApplicationTypes()
{
	_applicationTypes.push_back("aspx");
	_applicationTypes.push_back("asp");
	_applicationTypes.push_back("php");
	_applicationTypes.push_back("jsp");
}

/// <summary>
/// Create the mime types.
/// </summary>
void MimeType::CreateMimeTypes()
{
	typedef std::pair<std::string, std::string> headerPair;
	
	_mimeTypes.insert(headerPair("3dm", "x-world/x-3dmf"));
	_mimeTypes.insert(headerPair("3dmf", "x-world/x-3dmf"));
	_mimeTypes.insert(headerPair("7z", "application/x-7z-compressed"));
	_mimeTypes.insert(headerPair("aspx", "text/html"));
	_mimeTypes.insert(headerPair("asp", "text/html"));
	_mimeTypes.insert(headerPair("php", "text/html"));
	_mimeTypes.insert(headerPair("jsp", "text/html"));
	_mimeTypes.insert(headerPair("a", "application/octet-stream"));
	_mimeTypes.insert(headerPair("aab", "application/x-authorware-bin"));
	_mimeTypes.insert(headerPair("aam", "application/x-authorware-map"));
	_mimeTypes.insert(headerPair("aas", "application/x-authorware-seg"));
	_mimeTypes.insert(headerPair("abc", "text/vnd.abc"));
	_mimeTypes.insert(headerPair("acgi", "text/html"));
	_mimeTypes.insert(headerPair("afl", "video/animaflex"));
	_mimeTypes.insert(headerPair("ai", "application/postscript"));
	_mimeTypes.insert(headerPair("aif", "audio/x-aiff"));
	_mimeTypes.insert(headerPair("aifc", "audio/x-aiff"));
	_mimeTypes.insert(headerPair("aiff", "audio/x-aiff"));
	_mimeTypes.insert(headerPair("aim", "application/x-aim"));
	_mimeTypes.insert(headerPair("aip", "text/x-audiosoft-intra"));
	_mimeTypes.insert(headerPair("ani", "application/x-navi-animation"));
	_mimeTypes.insert(headerPair("aos", "application/x-nokia-9000-communicator-add-on-software"));
	_mimeTypes.insert(headerPair("aps", "application/mime"));
	_mimeTypes.insert(headerPair("arc", "application/octet-stream"));
	_mimeTypes.insert(headerPair("arj", "application/octet-stream"));
	_mimeTypes.insert(headerPair("art", "image/x-jg"));
	_mimeTypes.insert(headerPair("asf", "video/x-ms-asf"));
	_mimeTypes.insert(headerPair("asm", "text/x-asm"));
	_mimeTypes.insert(headerPair("asx", "video/x-ms-asf"));
	_mimeTypes.insert(headerPair("au", "audio/basic"));
	_mimeTypes.insert(headerPair("avi", "video/x-msvideo"));
	_mimeTypes.insert(headerPair("avs", "video/avs-video"));
	_mimeTypes.insert(headerPair("bcpio", "application/x-bcpio"));
	_mimeTypes.insert(headerPair("bin", "application/octet-stream"));
	_mimeTypes.insert(headerPair("bm", "image/bmp"));
	_mimeTypes.insert(headerPair("bmp", "image/bmp"));
	_mimeTypes.insert(headerPair("boo", "application/book"));
	_mimeTypes.insert(headerPair("book", "application/book"));
	_mimeTypes.insert(headerPair("boz", "application/x-bzip2"));
	_mimeTypes.insert(headerPair("bsh", "application/x-bsh"));
	_mimeTypes.insert(headerPair("bz", "application/x-bzip"));
	_mimeTypes.insert(headerPair("bz2", "application/x-bzip2"));
	_mimeTypes.insert(headerPair("chm", "application/octet-stream"));
	_mimeTypes.insert(headerPair("css", "text/css"));
	_mimeTypes.insert(headerPair("cer", "application/x-x509-ca-cert"));
	_mimeTypes.insert(headerPair("crt", "application/x-x509-ca-cert"));
	_mimeTypes.insert(headerPair("crl", "application/pkcs-crl"));
	_mimeTypes.insert(headerPair("c", "text/plain"));
	_mimeTypes.insert(headerPair("c++", "text/plain"));
	_mimeTypes.insert(headerPair("cat", "application/vnd.ms-pki.seccat"));
	_mimeTypes.insert(headerPair("cc", "text/plain"));
	_mimeTypes.insert(headerPair("ccad", "application/clariscad"));
	_mimeTypes.insert(headerPair("cco", "application/x-cocoa"));
	_mimeTypes.insert(headerPair("cdf", "application/x-netcdf"));
	_mimeTypes.insert(headerPair("cha", "application/x-chat"));
	_mimeTypes.insert(headerPair("chat", "application/x-chat"));
	_mimeTypes.insert(headerPair("class", "application/x-java-class"));
	_mimeTypes.insert(headerPair("com", "application/octet-stream"));
	_mimeTypes.insert(headerPair("conf", "text/plain"));
	_mimeTypes.insert(headerPair("cpio", "application/x-cpio"));
	_mimeTypes.insert(headerPair("cpp", "text/plain"));
	_mimeTypes.insert(headerPair("cpt", "application/x-compactpro"));
	_mimeTypes.insert(headerPair("csh", "application/x-csh"));
	_mimeTypes.insert(headerPair("cxx", "text/plain"));
	_mimeTypes.insert(headerPair("dll", "application/octet-stream"));
	_mimeTypes.insert(headerPair("der", "application/x-x509-ca-cert"));
	_mimeTypes.insert(headerPair("doc", "application/vnd.ms-word"));
	_mimeTypes.insert(headerPair("dot", "application/msword"));
	_mimeTypes.insert(headerPair("docx", "application/vnd.ms-word"));
	_mimeTypes.insert(headerPair("dcr", "application/x-director"));
	_mimeTypes.insert(headerPair("deepv", "application/x-deepv"));
	_mimeTypes.insert(headerPair("def", "text/plain"));
	_mimeTypes.insert(headerPair("dif", "video/x-dv"));
	_mimeTypes.insert(headerPair("dir", "application/x-director"));
	_mimeTypes.insert(headerPair("dl", "video/x-dl"));
	_mimeTypes.insert(headerPair("dp", "application/commonground"));
	_mimeTypes.insert(headerPair("drw", "application/drafting"));
	_mimeTypes.insert(headerPair("dump", "application/octet-stream"));
	_mimeTypes.insert(headerPair("dv", "video/x-dv"));
	_mimeTypes.insert(headerPair("dvi", "application/x-dvi"));
	_mimeTypes.insert(headerPair("dwf", "model/vnd.dwf"));
	_mimeTypes.insert(headerPair("dwg", "image/vnd.dwg"));
	_mimeTypes.insert(headerPair("dxf", "image/vnd.dwg"));
	_mimeTypes.insert(headerPair("dxr", "application/x-director"));
	_mimeTypes.insert(headerPair("exe", "application/octet-stream"));
	_mimeTypes.insert(headerPair("eml", "text/plain"));
	_mimeTypes.insert(headerPair("el", "text/x-script.elisp"));
	_mimeTypes.insert(headerPair("elc", "application/x-bytecode.elisp"));
	_mimeTypes.insert(headerPair("env", "application/x-envoy"));
	_mimeTypes.insert(headerPair("eps", "application/postscript"));
	_mimeTypes.insert(headerPair("es", "application/x-esrehber"));
	_mimeTypes.insert(headerPair("etx", "text/x-setext"));
	_mimeTypes.insert(headerPair("evy", "application/x-envoy"));
	_mimeTypes.insert(headerPair("f", "text/x-fortran"));
	_mimeTypes.insert(headerPair("f77", "text/x-fortran"));
	_mimeTypes.insert(headerPair("f90", "text/x-fortran"));
	_mimeTypes.insert(headerPair("fdf", "application/vnd.fdf"));
	_mimeTypes.insert(headerPair("fif", "image/fif"));
	_mimeTypes.insert(headerPair("fli", "video/x-fli"));
	_mimeTypes.insert(headerPair("flo", "image/florian"));
	_mimeTypes.insert(headerPair("flx", "text/vnd.fmi.flexstor"));
	_mimeTypes.insert(headerPair("fmf", "video/x-atomic3d-feature"));
	_mimeTypes.insert(headerPair("for", "text/x-fortran"));
	_mimeTypes.insert(headerPair("fpx", "image/vnd.fpx"));
	_mimeTypes.insert(headerPair("frl", "application/freeloader"));
	_mimeTypes.insert(headerPair("funk", "audio/make"));
	_mimeTypes.insert(headerPair("gz", "application/x-gzip"));
	_mimeTypes.insert(headerPair("gif", "image/gif"));
	_mimeTypes.insert(headerPair("g", "text/plain"));
	_mimeTypes.insert(headerPair("g3", "image/g3fax"));
	_mimeTypes.insert(headerPair("gl", "video/x-gl"));
	_mimeTypes.insert(headerPair("gsd", "audio/x-gsm"));
	_mimeTypes.insert(headerPair("gsm", "audio/x-gsm"));
	_mimeTypes.insert(headerPair("gsp", "application/x-gsp"));
	_mimeTypes.insert(headerPair("gss", "application/x-gss"));
	_mimeTypes.insert(headerPair("gtar", "application/x-gtar"));
	_mimeTypes.insert(headerPair("gzip", "application/x-gzip"));
	_mimeTypes.insert(headerPair("htm", "text/html"));
	_mimeTypes.insert(headerPair("html", "text/html"));
	_mimeTypes.insert(headerPair("htmls", "text/html"));
	_mimeTypes.insert(headerPair("h", "text/plain"));
	_mimeTypes.insert(headerPair("hpp", "text/plain"));
	_mimeTypes.insert(headerPair("hdf", "application/x-hdf"));
	_mimeTypes.insert(headerPair("help", "application/x-helpfile"));
	_mimeTypes.insert(headerPair("hgl", "application/vnd.hp-hpgl"));
	_mimeTypes.insert(headerPair("hh", "text/plain"));
	_mimeTypes.insert(headerPair("hlp", "application/hlp"));
	_mimeTypes.insert(headerPair("hpg", "application/vnd.hp-hpgl"));
	_mimeTypes.insert(headerPair("hpgl", "application/vnd.hp-hpgl"));
	_mimeTypes.insert(headerPair("hqx", "application/binhex"));
	_mimeTypes.insert(headerPair("hta", "application/hta"));
	_mimeTypes.insert(headerPair("htc", "text/x-component"));
	_mimeTypes.insert(headerPair("htt", "text/webviewhtml"));
	_mimeTypes.insert(headerPair("htx", "text/html"));
	_mimeTypes.insert(headerPair("ico", "image/x-icon"));
	_mimeTypes.insert(headerPair("ical", "text/calendar"));
	_mimeTypes.insert(headerPair("ics", "text/calendar"));
	_mimeTypes.insert(headerPair("ifb", "text/calendar"));
	_mimeTypes.insert(headerPair("icalendar", "text/calendar"));
	_mimeTypes.insert(headerPair("ice", "x-conference/x-cooltalk"));
	_mimeTypes.insert(headerPair("idc", "text/plain"));
	_mimeTypes.insert(headerPair("ief", "image/ief"));
	_mimeTypes.insert(headerPair("iefs", "image/ief"));
	_mimeTypes.insert(headerPair("iges", "application/iges"));
	_mimeTypes.insert(headerPair("igs", "application/iges"));
	_mimeTypes.insert(headerPair("ima", "application/x-ima"));
	_mimeTypes.insert(headerPair("imap", "application/x-httpd-imap"));
	_mimeTypes.insert(headerPair("inf", "application/inf"));
	_mimeTypes.insert(headerPair("ins", "application/x-internett-signup"));
	_mimeTypes.insert(headerPair("ip", "application/x-ip2"));
	_mimeTypes.insert(headerPair("isu", "video/x-isvideo"));
	_mimeTypes.insert(headerPair("it", "audio/it"));
	_mimeTypes.insert(headerPair("iv", "application/x-inventor"));
	_mimeTypes.insert(headerPair("ivr", "i-world/i-vrml"));
	_mimeTypes.insert(headerPair("ivy", "application/x-livescreen"));
	_mimeTypes.insert(headerPair("jpe", "image/jpeg"));
	_mimeTypes.insert(headerPair("jpg", "image/jpeg"));
	_mimeTypes.insert(headerPair("jpeg", "image/jpeg"));
	_mimeTypes.insert(headerPair("js", "application/x-javascript"));
	_mimeTypes.insert(headerPair("jsx", "text/jscript"));
	_mimeTypes.insert(headerPair("java", "text/plain"));
	_mimeTypes.insert(headerPair("jam", "audio/x-jam"));
	_mimeTypes.insert(headerPair("jav", "text/plain"));
	_mimeTypes.insert(headerPair("jcm", "application/x-java-commerce"));
	_mimeTypes.insert(headerPair("jfif", "image/jpeg"));
	_mimeTypes.insert(headerPair("jfif-tbnl", "image/jpeg"));
	_mimeTypes.insert(headerPair("jps", "image/x-jps"));
	_mimeTypes.insert(headerPair("jut", "image/jutvision"));
	_mimeTypes.insert(headerPair("kar", "audio/midi"));
	_mimeTypes.insert(headerPair("ksh", "application/x-ksh"));
	_mimeTypes.insert(headerPair("log", "text/plain"));
	_mimeTypes.insert(headerPair("la", "audio/nspaudio"));
	_mimeTypes.insert(headerPair("lam", "audio/x-liveaudio"));
	_mimeTypes.insert(headerPair("latex", "application/x-latex"));
	_mimeTypes.insert(headerPair("lha", "application/octet-stream"));
	_mimeTypes.insert(headerPair("lhx", "application/octet-stream"));
	_mimeTypes.insert(headerPair("list", "text/plain"));
	_mimeTypes.insert(headerPair("lma", "audio/nspaudio"));
	_mimeTypes.insert(headerPair("lsp", "application/x-lisp"));
	_mimeTypes.insert(headerPair("lst", "text/plain"));
	_mimeTypes.insert(headerPair("lsx", "text/x-la-asf"));
	_mimeTypes.insert(headerPair("ltx", "application/x-latex"));
	_mimeTypes.insert(headerPair("lzh", "application/octet-stream"));
	_mimeTypes.insert(headerPair("lzx", "application/octet-stream"));
	_mimeTypes.insert(headerPair("mid", "audio/mid"));
	_mimeTypes.insert(headerPair("midi", "audio/mid"));
	_mimeTypes.insert(headerPair("mov", "video/quicktime"));
	_mimeTypes.insert(headerPair("movie", "video/x-sgi-movie"));
	_mimeTypes.insert(headerPair("mp2", "video/mpeg"));
	_mimeTypes.insert(headerPair("mp3", "video/mpeg"));
	_mimeTypes.insert(headerPair("mpa", "video/mpeg"));
	_mimeTypes.insert(headerPair("mpe", "video/mpeg"));
	_mimeTypes.insert(headerPair("mpeg", "video/mpeg"));
	_mimeTypes.insert(headerPair("mpg", "video/mpeg"));
	_mimeTypes.insert(headerPair("mpv2", "video/mpeg"));
	_mimeTypes.insert(headerPair("mht", "message/rfc822"));
	_mimeTypes.insert(headerPair("mhtml", "message/rfc822"));
	_mimeTypes.insert(headerPair("m", "text/plain"));
	_mimeTypes.insert(headerPair("m1v", "video/mpeg"));
	_mimeTypes.insert(headerPair("m2a", "audio/mpeg"));
	_mimeTypes.insert(headerPair("m2v", "video/mpeg"));
	_mimeTypes.insert(headerPair("m3u", "audio/x-mpequrl"));
	_mimeTypes.insert(headerPair("man", "application/x-troff-man"));
	_mimeTypes.insert(headerPair("map", "application/x-navimap"));
	_mimeTypes.insert(headerPair("mar", "text/plain"));
	_mimeTypes.insert(headerPair("mbd", "application/mbedlet"));
	_mimeTypes.insert(headerPair("mc$", "application/x-magic-cap-package-1.0"));
	_mimeTypes.insert(headerPair("mcd", "application/mcad"));
	_mimeTypes.insert(headerPair("mcf", "image/vasa"));
	_mimeTypes.insert(headerPair("mcp", "application/netmc"));
	_mimeTypes.insert(headerPair("me", "application/x-troff-me"));
	_mimeTypes.insert(headerPair("mif", "application/x-frame"));
	_mimeTypes.insert(headerPair("mime", "message/rfc822"));
	_mimeTypes.insert(headerPair("mjf", "audio/x-vnd.audioexplosion.mjuicemediafile"));
	_mimeTypes.insert(headerPair("mjpg", "video/x-motion-jpeg"));
	_mimeTypes.insert(headerPair("mm", "application/base64"));
	_mimeTypes.insert(headerPair("mme", "application/base64"));
	_mimeTypes.insert(headerPair("mod", "audio/mod"));
	_mimeTypes.insert(headerPair("moov", "video/quicktime"));
	_mimeTypes.insert(headerPair("mpc", "application/x-project"));
	_mimeTypes.insert(headerPair("mpga", "audio/mpeg"));
	_mimeTypes.insert(headerPair("mpp", "application/vnd.ms-project"));
	_mimeTypes.insert(headerPair("mpt", "application/x-project"));
	_mimeTypes.insert(headerPair("mpv", "application/x-project"));
	_mimeTypes.insert(headerPair("mpx", "application/x-project"));
	_mimeTypes.insert(headerPair("mrc", "application/marc"));
	_mimeTypes.insert(headerPair("ms", "application/x-troff-ms"));
	_mimeTypes.insert(headerPair("mv", "video/x-sgi-movie"));
	_mimeTypes.insert(headerPair("my", "audio/make"));
	_mimeTypes.insert(headerPair("mzz", "application/x-vnd.audioexplosion.mzz"));
	_mimeTypes.insert(headerPair("nap", "image/naplps"));
	_mimeTypes.insert(headerPair("naplps", "image/naplps"));
	_mimeTypes.insert(headerPair("nc", "application/x-netcdf"));
	_mimeTypes.insert(headerPair("ncm", "application/vnd.nokia.configuration-message"));
	_mimeTypes.insert(headerPair("nif", "image/x-niff"));
	_mimeTypes.insert(headerPair("niff", "image/x-niff"));
	_mimeTypes.insert(headerPair("nix", "application/x-mix-transfer"));
	_mimeTypes.insert(headerPair("nsc", "application/x-conference"));
	_mimeTypes.insert(headerPair("nvd", "application/x-navidoc"));
	_mimeTypes.insert(headerPair("o", "application/octet-stream"));
	_mimeTypes.insert(headerPair("oda", "application/oda"));
	_mimeTypes.insert(headerPair("omc", "application/x-omc"));
	_mimeTypes.insert(headerPair("omcd", "application/x-omcdatamaker"));
	_mimeTypes.insert(headerPair("omcr", "application/x-omcregerator"));
	_mimeTypes.insert(headerPair("pdf", "application/pdf"));
	_mimeTypes.insert(headerPair("png", "image/png"));
	_mimeTypes.insert(headerPair("pnz", "image/png"));
	_mimeTypes.insert(headerPair("p12", "application/x-pkcs12"));
	_mimeTypes.insert(headerPair("pfx", "application/x-pkcs12"));
	_mimeTypes.insert(headerPair("p", "text/x-pascal"));
	_mimeTypes.insert(headerPair("p10", "application/x-pkcs10"));
	_mimeTypes.insert(headerPair("p7a", "application/x-pkcs7-signature"));
	_mimeTypes.insert(headerPair("p7c", "application/pkcs7-mime"));
	_mimeTypes.insert(headerPair("p7m", "application/pkcs7-mime"));
	_mimeTypes.insert(headerPair("p7r", "application/x-pkcs7-certreqresp"));
	_mimeTypes.insert(headerPair("p7s", "application/pkcs7-signature"));
	_mimeTypes.insert(headerPair("part", "application/pro_eng"));
	_mimeTypes.insert(headerPair("pas", "text/pascal"));
	_mimeTypes.insert(headerPair("pbm", "image/x-portable-bitmap"));
	_mimeTypes.insert(headerPair("pcl", "application/vnd.hp-pcl"));
	_mimeTypes.insert(headerPair("pct", "image/x-pict"));
	_mimeTypes.insert(headerPair("pcx", "image/x-pcx"));
	_mimeTypes.insert(headerPair("pdb", "chemical/x-pdb"));
	_mimeTypes.insert(headerPair("pfunk", "audio/make"));
	_mimeTypes.insert(headerPair("pgm", "image/x-portable-graymap"));
	_mimeTypes.insert(headerPair("pic", "image/pict"));
	_mimeTypes.insert(headerPair("pict", "image/pict"));
	_mimeTypes.insert(headerPair("pkg", "application/x-newton-compatible-pkg"));
	_mimeTypes.insert(headerPair("pko", "application/vnd.ms-pki.pko"));
	_mimeTypes.insert(headerPair("pl", "text/x-script.perl"));
	_mimeTypes.insert(headerPair("plx", "application/x-pixclscript"));
	_mimeTypes.insert(headerPair("pm", "text/x-script.perl-module"));
	_mimeTypes.insert(headerPair("pm4", "application/x-pagemaker"));
	_mimeTypes.insert(headerPair("pm5", "application/x-pagemaker"));
	_mimeTypes.insert(headerPair("pnm", "image/x-portable-anymap"));
	_mimeTypes.insert(headerPair("pot", "application/vnd.ms-powerpoint"));
	_mimeTypes.insert(headerPair("pov", "model/x-pov"));
	_mimeTypes.insert(headerPair("ppa", "application/vnd.ms-powerpoint"));
	_mimeTypes.insert(headerPair("ppm", "image/x-portable-pixmap"));
	_mimeTypes.insert(headerPair("pps", "application/vnd.ms-powerpoint"));
	_mimeTypes.insert(headerPair("ppt", "application/powerpoint"));
	_mimeTypes.insert(headerPair("ppz", "application/mspowerpoint"));
	_mimeTypes.insert(headerPair("pre", "application/x-freelance"));
	_mimeTypes.insert(headerPair("prt", "application/pro_eng"));
	_mimeTypes.insert(headerPair("ps", "application/postscript"));
	_mimeTypes.insert(headerPair("psd", "application/octet-stream"));
	_mimeTypes.insert(headerPair("pvu", "paleovu/x-pv"));
	_mimeTypes.insert(headerPair("pwz", "application/vnd.ms-powerpoint"));
	_mimeTypes.insert(headerPair("py", "text/x-script.phyton"));
	_mimeTypes.insert(headerPair("pyc", "applicaiton/x-bytecode.python"));
	_mimeTypes.insert(headerPair("qcp", "audio/vnd.qcelp"));
	_mimeTypes.insert(headerPair("qd3", "x-world/x-3dmf"));
	_mimeTypes.insert(headerPair("qd3d", "x-world/x-3dmf"));
	_mimeTypes.insert(headerPair("qif", "image/x-quicktime"));
	_mimeTypes.insert(headerPair("qt", "video/quicktime"));
	_mimeTypes.insert(headerPair("qtc", "video/x-qtc"));
	_mimeTypes.insert(headerPair("qti", "image/x-quicktime"));
	_mimeTypes.insert(headerPair("qtif", "image/x-quicktime"));
	_mimeTypes.insert(headerPair("rss", "application/rss+xml"));
	_mimeTypes.insert(headerPair("rtf", "text/richtext"));
	_mimeTypes.insert(headerPair("rtx", "text/richtext"));
	_mimeTypes.insert(headerPair("ra", "audio/x-realaudio"));
	_mimeTypes.insert(headerPair("ram", "audio/x-pn-realaudio"));
	_mimeTypes.insert(headerPair("ras", "image/x-cmu-raster"));
	_mimeTypes.insert(headerPair("rast", "image/cmu-raster"));
	_mimeTypes.insert(headerPair("rexx", "text/x-script.rexx"));
	_mimeTypes.insert(headerPair("rf", "image/vnd.rn-realflash"));
	_mimeTypes.insert(headerPair("rgb", "image/x-rgb"));
	_mimeTypes.insert(headerPair("rm", "audio/x-pn-realaudio"));
	_mimeTypes.insert(headerPair("rmi", "audio/mid"));
	_mimeTypes.insert(headerPair("rmm", "audio/x-pn-realaudio"));
	_mimeTypes.insert(headerPair("rmp", "audio/x-pn-realaudio"));
	_mimeTypes.insert(headerPair("rng", "application/ringing-tones"));
	_mimeTypes.insert(headerPair("rnx", "application/vnd.rn-realplayer"));
	_mimeTypes.insert(headerPair("roff", "application/x-troff"));
	_mimeTypes.insert(headerPair("rp", "image/vnd.rn-realpix"));
	_mimeTypes.insert(headerPair("rpm", "audio/x-pn-realaudio-plugin"));
	_mimeTypes.insert(headerPair("rt", "text/richtext"));
	_mimeTypes.insert(headerPair("rv", "video/vnd.rn-realvideo"));
	_mimeTypes.insert(headerPair("swf", "application/x-shockwave-flash"));
	_mimeTypes.insert(headerPair("s", "text/x-asm"));
	_mimeTypes.insert(headerPair("s3m", "audio/s3m"));
	_mimeTypes.insert(headerPair("saveme", "application/octet-stream"));
	_mimeTypes.insert(headerPair("sbk", "application/x-tbook"));
	_mimeTypes.insert(headerPair("scm", "video/x-scm"));
	_mimeTypes.insert(headerPair("sdml", "text/plain"));
	_mimeTypes.insert(headerPair("sdp", "application/x-sdp"));
	_mimeTypes.insert(headerPair("sdr", "application/sounder"));
	_mimeTypes.insert(headerPair("sea", "application/x-sea"));
	_mimeTypes.insert(headerPair("set", "application/set"));
	_mimeTypes.insert(headerPair("sgm", "text/x-sgml"));
	_mimeTypes.insert(headerPair("sgml", "text/x-sgml"));
	_mimeTypes.insert(headerPair("sh", "text/x-script.sh"));
	_mimeTypes.insert(headerPair("shar", "application/x-bsh"));
	_mimeTypes.insert(headerPair("shtml", "text/html"));
	_mimeTypes.insert(headerPair("sid", "audio/x-psid"));
	_mimeTypes.insert(headerPair("sit", "application/x-sit"));
	_mimeTypes.insert(headerPair("skd", "application/x-koan"));
	_mimeTypes.insert(headerPair("skm", "application/x-koan"));
	_mimeTypes.insert(headerPair("skp", "application/x-koan"));
	_mimeTypes.insert(headerPair("skt", "application/x-koan"));
	_mimeTypes.insert(headerPair("sl", "application/x-seelogo"));
	_mimeTypes.insert(headerPair("smi", "application/smil"));
	_mimeTypes.insert(headerPair("smil", "application/smil"));
	_mimeTypes.insert(headerPair("snd", "audio/basic"));
	_mimeTypes.insert(headerPair("sol", "application/solids"));
	_mimeTypes.insert(headerPair("spc", "application/x-pkcs7-certificates"));
	_mimeTypes.insert(headerPair("spl", "application/futuresplash"));
	_mimeTypes.insert(headerPair("spr", "application/x-sprite"));
	_mimeTypes.insert(headerPair("sprite", "application/x-sprite"));
	_mimeTypes.insert(headerPair("src", "application/x-wais-source"));
	_mimeTypes.insert(headerPair("ssi", "text/x-server-parsed-html"));
	_mimeTypes.insert(headerPair("ssm", "application/streamingmedia"));
	_mimeTypes.insert(headerPair("sst", "application/vnd.ms-pki.certstore"));
	_mimeTypes.insert(headerPair("step", "application/step"));
	_mimeTypes.insert(headerPair("stl", "application/sla"));
	_mimeTypes.insert(headerPair("stp", "application/step"));
	_mimeTypes.insert(headerPair("sv4cpio", "application/x-sv4cpio"));
	_mimeTypes.insert(headerPair("sv4crc", "application/x-sv4crc"));
	_mimeTypes.insert(headerPair("svf", "image/x-dwg"));
	_mimeTypes.insert(headerPair("svr", "application/x-world"));
	_mimeTypes.insert(headerPair("txt", "text/plain"));
	_mimeTypes.insert(headerPair("tgz", "application/x-compressed"));
	_mimeTypes.insert(headerPair("tif", "image/tiff"));
	_mimeTypes.insert(headerPair("tiff", "image/tiff"));
	_mimeTypes.insert(headerPair("t", "application/x-troff"));
	_mimeTypes.insert(headerPair("talk", "text/x-speech"));
	_mimeTypes.insert(headerPair("tar", "application/x-tar"));
	_mimeTypes.insert(headerPair("tbk", "application/x-tbook"));
	_mimeTypes.insert(headerPair("tcl", "text/x-script.tcl"));
	_mimeTypes.insert(headerPair("tcsh", "text/x-script.tcsh"));
	_mimeTypes.insert(headerPair("tex", "application/x-tex"));
	_mimeTypes.insert(headerPair("texi", "application/x-texinfo"));
	_mimeTypes.insert(headerPair("texinfo", "application/x-texinfo"));
	_mimeTypes.insert(headerPair("text", "text/plain"));
	_mimeTypes.insert(headerPair("tr", "application/x-troff"));
	_mimeTypes.insert(headerPair("tsi", "audio/tsp-audio"));
	_mimeTypes.insert(headerPair("tsp", "audio/tsplayer"));
	_mimeTypes.insert(headerPair("tsv", "text/tab-separated-values"));
	_mimeTypes.insert(headerPair("turbot", "image/florian"));
	_mimeTypes.insert(headerPair("uil", "text/x-uil"));
	_mimeTypes.insert(headerPair("uni", "text/uri-list"));
	_mimeTypes.insert(headerPair("unis", "text/uri-list"));
	_mimeTypes.insert(headerPair("unv", "application/i-deas"));
	_mimeTypes.insert(headerPair("uri", "text/uri-list"));
	_mimeTypes.insert(headerPair("uris", "text/uri-list"));
	_mimeTypes.insert(headerPair("ustar", "application/x-ustar"));
	_mimeTypes.insert(headerPair("uu", "text/x-uuencode"));
	_mimeTypes.insert(headerPair("uue", "text/x-uuencode"));
	_mimeTypes.insert(headerPair("vcd", "application/x-cdlink"));
	_mimeTypes.insert(headerPair("vcs", "text/x-vcalendar"));
	_mimeTypes.insert(headerPair("vda", "application/vda"));
	_mimeTypes.insert(headerPair("vdo", "video/vdo"));
	_mimeTypes.insert(headerPair("vew", "application/groupwise"));
	_mimeTypes.insert(headerPair("viv", "video/vivo"));
	_mimeTypes.insert(headerPair("vivo", "video/vivo"));
	_mimeTypes.insert(headerPair("vmd", "application/vocaltec-media-desc"));
	_mimeTypes.insert(headerPair("vmf", "application/vocaltec-media-file"));
	_mimeTypes.insert(headerPair("voc", "audio/x-voc"));
	_mimeTypes.insert(headerPair("vos", "video/vosaic"));
	_mimeTypes.insert(headerPair("vox", "audio/voxware"));
	_mimeTypes.insert(headerPair("vqe", "audio/x-twinvq-plugin"));
	_mimeTypes.insert(headerPair("vqf", "audio/x-twinvq"));
	_mimeTypes.insert(headerPair("vql", "audio/x-twinvq-plugin"));
	_mimeTypes.insert(headerPair("vrml", "application/x-vrml"));
	_mimeTypes.insert(headerPair("vrt", "x-world/x-vrt"));
	_mimeTypes.insert(headerPair("vsd", "application/x-visio"));
	_mimeTypes.insert(headerPair("vst", "application/x-visio"));
	_mimeTypes.insert(headerPair("vsw", "application/x-visio"));
	_mimeTypes.insert(headerPair("wav", "audio/wav"));
	_mimeTypes.insert(headerPair("wmv", "video/x-ms-wmv"));
	_mimeTypes.insert(headerPair("w60", "application/wordperfect6.0"));
	_mimeTypes.insert(headerPair("w61", "application/wordperfect6.1"));
	_mimeTypes.insert(headerPair("w6w", "application/msword"));
	_mimeTypes.insert(headerPair("wb1", "application/x-qpro"));
	_mimeTypes.insert(headerPair("wbmp", "image/vnd.wap.wbmp"));
	_mimeTypes.insert(headerPair("web", "application/vnd.xara"));
	_mimeTypes.insert(headerPair("wiz", "application/msword"));
	_mimeTypes.insert(headerPair("wk1", "application/x-123"));
	_mimeTypes.insert(headerPair("wmf", "windows/metafile"));
	_mimeTypes.insert(headerPair("wml", "text/vnd.wap.wml"));
	_mimeTypes.insert(headerPair("wmlc", "application/vnd.wap.wmlc"));
	_mimeTypes.insert(headerPair("wmls", "text/vnd.wap.wmlscript"));
	_mimeTypes.insert(headerPair("wmlsc", "application/vnd.wap.wmlscriptc"));
	_mimeTypes.insert(headerPair("word", "application/msword"));
	_mimeTypes.insert(headerPair("wp", "application/wordperfect"));
	_mimeTypes.insert(headerPair("wp5", "application/wordperfect"));
	_mimeTypes.insert(headerPair("wp6", "application/wordperfect"));
	_mimeTypes.insert(headerPair("wpd", "application/wordperfect"));
	_mimeTypes.insert(headerPair("wq1", "application/x-lotus"));
	_mimeTypes.insert(headerPair("wri", "application/mswrite"));
	_mimeTypes.insert(headerPair("wrl", "model/vrml"));
	_mimeTypes.insert(headerPair("wrz", "model/vrml"));
	_mimeTypes.insert(headerPair("wsc", "text/scriplet"));
	_mimeTypes.insert(headerPair("wsrc", "application/x-wais-source"));
	_mimeTypes.insert(headerPair("wtk", "application/x-wintalk"));
	_mimeTypes.insert(headerPair("wsdl", "text/xml"));
	_mimeTypes.insert(headerPair("xml", "text/xml"));
	_mimeTypes.insert(headerPair("xslt", "text/xml"));
	_mimeTypes.insert(headerPair("xsd", "text/xml"));
	_mimeTypes.insert(headerPair("xsf", "text/xml"));
	_mimeTypes.insert(headerPair("xsl", "text/xml"));
	_mimeTypes.insert(headerPair("xlm", "application/vnd.ms-excel"));
	_mimeTypes.insert(headerPair("xls", "application/vnd.ms-excel"));
	_mimeTypes.insert(headerPair("xlsx", "application/vnd.ms-excel"));
	_mimeTypes.insert(headerPair("xlt", "application/vnd.ms-excel"));
	_mimeTypes.insert(headerPair("xbm", "image/xbm"));
	_mimeTypes.insert(headerPair("xdr", "video/x-amt-demorun"));
	_mimeTypes.insert(headerPair("xgz", "xgl/drawing"));
	_mimeTypes.insert(headerPair("xif", "image/vnd.xiff"));
	_mimeTypes.insert(headerPair("xl", "application/excel"));
	_mimeTypes.insert(headerPair("xla", "application/excel"));
	_mimeTypes.insert(headerPair("xlb", "application/excel"));
	_mimeTypes.insert(headerPair("xlc", "application/excel"));
	_mimeTypes.insert(headerPair("xld", "application/excel"));
	_mimeTypes.insert(headerPair("xlk", "application/excel"));
	_mimeTypes.insert(headerPair("xll", "application/excel"));
	_mimeTypes.insert(headerPair("xlv", "application/excel"));
	_mimeTypes.insert(headerPair("xlw", "application/excel"));
	_mimeTypes.insert(headerPair("xm", "audio/xm"));
	_mimeTypes.insert(headerPair("xmz", "xgl/movie"));
	_mimeTypes.insert(headerPair("xpix", "application/x-vnd.ls-xpix"));
	_mimeTypes.insert(headerPair("xpm", "image/x-xpixmap"));
	_mimeTypes.insert(headerPair("x-png", "image/png"));
	_mimeTypes.insert(headerPair("xsr", "video/x-amt-showrun"));
	_mimeTypes.insert(headerPair("xwd", "image/x-xwd"));
	_mimeTypes.insert(headerPair("xyz", "chemical/x-pdb"));
	_mimeTypes.insert(headerPair("z", "application/x-compress"));
	_mimeTypes.insert(headerPair("zip", "application/x-zip-compressed"));
	_mimeTypes.insert(headerPair("zoo", "application/octet-stream"));
	_mimeTypes.insert(headerPair("zsh", "text/x-script.zsh"));
}