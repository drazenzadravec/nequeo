/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;

namespace Nequeo.Net.Mime
{
    /// <summary>
    /// Mime types.
    /// </summary>
    public class MimeType
    {
        private static IDictionary<string, string> _mimeTypes = new Dictionary<string, string>();
        private static List<string> _applicationTypes = new List<string>();

        /// <summary>
        /// Is the mime type an application hosting type.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>True if application hosting type; else false.</returns>
        public static bool IsApplicationType(string extension)
        {
            // If no application types exist.
            if (_applicationTypes.Count < 1)
            {
                _applicationTypes.Add("aspx");
                _applicationTypes.Add("asp");
                _applicationTypes.Add("php");
                _applicationTypes.Add("jsp");
            }

            IEnumerable<string> appTypes = _applicationTypes.Where(u => u.ToLower() == extension.Trim(new char[] { '.' }).ToLower());
            if (appTypes.Count() > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the mime type for the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The mime type.</returns>
        public static string GetMimeType(string extension)
        {
            // If no mime types exist.
            if (_mimeTypes.Count < 1)
            {
                _mimeTypes.Add("3dm", "x-world/x-3dmf");
                _mimeTypes.Add("3dmf", "x-world/x-3dmf");
                _mimeTypes.Add("7z", "application/x-7z-compressed");
                _mimeTypes.Add("aspx", "text/html");
                _mimeTypes.Add("asp", "text/html");
                _mimeTypes.Add("a", "application/octet-stream");
                _mimeTypes.Add("aab", "application/x-authorware-bin");
                _mimeTypes.Add("aam", "application/x-authorware-map");
                _mimeTypes.Add("aas", "application/x-authorware-seg");
                _mimeTypes.Add("abc", "text/vnd.abc");
                _mimeTypes.Add("acgi", "text/html");
                _mimeTypes.Add("afl", "video/animaflex");
                _mimeTypes.Add("ai", "application/postscript");
                _mimeTypes.Add("aif", "audio/x-aiff");
                _mimeTypes.Add("aifc", "audio/x-aiff");
                _mimeTypes.Add("aiff", "audio/x-aiff");
                _mimeTypes.Add("aim", "application/x-aim");
                _mimeTypes.Add("aip", "text/x-audiosoft-intra");
                _mimeTypes.Add("ani", "application/x-navi-animation");
                _mimeTypes.Add("aos", "application/x-nokia-9000-communicator-add-on-software");
                _mimeTypes.Add("aps", "application/mime");
                _mimeTypes.Add("arc", "application/octet-stream");
                _mimeTypes.Add("arj", "application/octet-stream");
                _mimeTypes.Add("art", "image/x-jg");
                _mimeTypes.Add("asf", "video/x-ms-asf");
                _mimeTypes.Add("asm", "text/x-asm");
                _mimeTypes.Add("asx", "video/x-ms-asf");
                _mimeTypes.Add("au", "audio/basic");
                _mimeTypes.Add("avi", "video/x-msvideo");
                _mimeTypes.Add("avs", "video/avs-video");
                _mimeTypes.Add("bcpio", "application/x-bcpio");
                _mimeTypes.Add("bin", "application/octet-stream");
                _mimeTypes.Add("bm", "image/bmp");
                _mimeTypes.Add("bmp", "image/bmp");
                _mimeTypes.Add("boo", "application/book");
                _mimeTypes.Add("book", "application/book");
                _mimeTypes.Add("boz", "application/x-bzip2");
                _mimeTypes.Add("bsh", "application/x-bsh");
                _mimeTypes.Add("bz", "application/x-bzip");
                _mimeTypes.Add("bz2", "application/x-bzip2");
                _mimeTypes.Add("chm", "application/octet-stream");
                _mimeTypes.Add("css", "text/css");
                _mimeTypes.Add("cer", "application/x-x509-ca-cert");
                _mimeTypes.Add("crt", "application/x-x509-ca-cert");
                _mimeTypes.Add("crl", "application/pkcs-crl");
                _mimeTypes.Add("c", "text/plain");
                _mimeTypes.Add("c++", "text/plain");
                _mimeTypes.Add("cat", "application/vnd.ms-pki.seccat");
                _mimeTypes.Add("cc", "text/plain");
                _mimeTypes.Add("ccad", "application/clariscad");
                _mimeTypes.Add("cco", "application/x-cocoa");
                _mimeTypes.Add("cdf", "application/x-netcdf");
                _mimeTypes.Add("cha", "application/x-chat");
                _mimeTypes.Add("chat", "application/x-chat");
                _mimeTypes.Add("class", "application/x-java-class");
                _mimeTypes.Add("com", "application/octet-stream");
                _mimeTypes.Add("conf", "text/plain");
                _mimeTypes.Add("cpio", "application/x-cpio");
                _mimeTypes.Add("cpp", "text/plain");
                _mimeTypes.Add("cpt", "application/x-compactpro");
                _mimeTypes.Add("csh", "application/x-csh");
                _mimeTypes.Add("cxx", "text/plain");
                _mimeTypes.Add("dll", "application/octet-stream");
                _mimeTypes.Add("der", "application/x-x509-ca-cert");
                _mimeTypes.Add("doc", "application/vnd.ms-word");
                _mimeTypes.Add("dot", "application/msword");
                _mimeTypes.Add("docx", "application/vnd.ms-word");
                _mimeTypes.Add("dcr", "application/x-director");
                _mimeTypes.Add("deepv", "application/x-deepv");
                _mimeTypes.Add("def", "text/plain");
                _mimeTypes.Add("dif", "video/x-dv");
                _mimeTypes.Add("dir", "application/x-director");
                _mimeTypes.Add("dl", "video/x-dl");
                _mimeTypes.Add("dp", "application/commonground");
                _mimeTypes.Add("drw", "application/drafting");
                _mimeTypes.Add("dump", "application/octet-stream");
                _mimeTypes.Add("dv", "video/x-dv");
                _mimeTypes.Add("dvi", "application/x-dvi");
                _mimeTypes.Add("dwf", "model/vnd.dwf");
                _mimeTypes.Add("dwg", "image/vnd.dwg");
                _mimeTypes.Add("dxf", "image/vnd.dwg");
                _mimeTypes.Add("dxr", "application/x-director");
                _mimeTypes.Add("exe", "application/octet-stream");
                _mimeTypes.Add("eml", "text/plain");
                _mimeTypes.Add("el", "text/x-script.elisp");
                _mimeTypes.Add("elc", "application/x-bytecode.elisp");
                _mimeTypes.Add("env", "application/x-envoy");
                _mimeTypes.Add("eps", "application/postscript");
                _mimeTypes.Add("es", "application/x-esrehber");
                _mimeTypes.Add("etx", "text/x-setext");
                _mimeTypes.Add("evy", "application/x-envoy");
                _mimeTypes.Add("f", "text/x-fortran");
                _mimeTypes.Add("f77", "text/x-fortran");
                _mimeTypes.Add("f90", "text/x-fortran");
                _mimeTypes.Add("fdf", "application/vnd.fdf");
                _mimeTypes.Add("fif", "image/fif");
                _mimeTypes.Add("fli", "video/x-fli");
                _mimeTypes.Add("flo", "image/florian");
                _mimeTypes.Add("flx", "text/vnd.fmi.flexstor");
                _mimeTypes.Add("fmf", "video/x-atomic3d-feature");
                _mimeTypes.Add("for", "text/x-fortran");
                _mimeTypes.Add("fpx", "image/vnd.fpx");
                _mimeTypes.Add("frl", "application/freeloader");
                _mimeTypes.Add("funk", "audio/make");
                _mimeTypes.Add("gz", "application/x-gzip");
                _mimeTypes.Add("gif", "image/gif");
                _mimeTypes.Add("g", "text/plain");
                _mimeTypes.Add("g3", "image/g3fax");
                _mimeTypes.Add("gl", "video/x-gl");
                _mimeTypes.Add("gsd", "audio/x-gsm");
                _mimeTypes.Add("gsm", "audio/x-gsm");
                _mimeTypes.Add("gsp", "application/x-gsp");
                _mimeTypes.Add("gss", "application/x-gss");
                _mimeTypes.Add("gtar", "application/x-gtar");
                _mimeTypes.Add("gzip", "application/x-gzip");
                _mimeTypes.Add("htm", "text/html");
                _mimeTypes.Add("html", "text/html");
                _mimeTypes.Add("htmls", "text/html");
                _mimeTypes.Add("h", "text/plain");
                _mimeTypes.Add("hpp", "text/plain");
                _mimeTypes.Add("hdf", "application/x-hdf");
                _mimeTypes.Add("help", "application/x-helpfile");
                _mimeTypes.Add("hgl", "application/vnd.hp-hpgl");
                _mimeTypes.Add("hh", "text/plain");
                _mimeTypes.Add("hlp", "application/hlp");
                _mimeTypes.Add("hpg", "application/vnd.hp-hpgl");
                _mimeTypes.Add("hpgl", "application/vnd.hp-hpgl");
                _mimeTypes.Add("hqx", "application/binhex");
                _mimeTypes.Add("hta", "application/hta");
                _mimeTypes.Add("htc", "text/x-component");
                _mimeTypes.Add("htt", "text/webviewhtml");
                _mimeTypes.Add("htx", "text/html");
                _mimeTypes.Add("ico", "image/x-icon");
                _mimeTypes.Add("ical", "text/calendar");
                _mimeTypes.Add("ics", "text/calendar");
                _mimeTypes.Add("ifb", "text/calendar");
                _mimeTypes.Add("icalendar", "text/calendar");
                _mimeTypes.Add("ice", "x-conference/x-cooltalk");
                _mimeTypes.Add("idc", "text/plain");
                _mimeTypes.Add("ief", "image/ief");
                _mimeTypes.Add("iefs", "image/ief");
                _mimeTypes.Add("iges", "application/iges");
                _mimeTypes.Add("igs", "application/iges");
                _mimeTypes.Add("ima", "application/x-ima");
                _mimeTypes.Add("imap", "application/x-httpd-imap");
                _mimeTypes.Add("inf", "application/inf");
                _mimeTypes.Add("ins", "application/x-internett-signup");
                _mimeTypes.Add("ip", "application/x-ip2");
                _mimeTypes.Add("isu", "video/x-isvideo");
                _mimeTypes.Add("it", "audio/it");
                _mimeTypes.Add("iv", "application/x-inventor");
                _mimeTypes.Add("ivr", "i-world/i-vrml");
                _mimeTypes.Add("ivy", "application/x-livescreen");
                _mimeTypes.Add("jpe", "image/jpeg");
                _mimeTypes.Add("jpg", "image/jpeg");
                _mimeTypes.Add("jpeg", "image/jpeg");
                _mimeTypes.Add("js", "application/x-javascript");
                _mimeTypes.Add("jsx", "text/jscript");
                _mimeTypes.Add("java", "text/plain");
                _mimeTypes.Add("jam", "audio/x-jam");
                _mimeTypes.Add("jav", "text/plain");
                _mimeTypes.Add("jcm", "application/x-java-commerce");
                _mimeTypes.Add("jfif", "image/jpeg");
                _mimeTypes.Add("jfif-tbnl", "image/jpeg");
                _mimeTypes.Add("jps", "image/x-jps");
                _mimeTypes.Add("jut", "image/jutvision");
                _mimeTypes.Add("kar", "audio/midi");
                _mimeTypes.Add("ksh", "application/x-ksh");
                _mimeTypes.Add("log", "text/plain");
                _mimeTypes.Add("la", "audio/nspaudio");
                _mimeTypes.Add("lam", "audio/x-liveaudio");
                _mimeTypes.Add("latex", "application/x-latex");
                _mimeTypes.Add("lha", "application/octet-stream");
                _mimeTypes.Add("lhx", "application/octet-stream");
                _mimeTypes.Add("list", "text/plain");
                _mimeTypes.Add("lma", "audio/nspaudio");
                _mimeTypes.Add("lsp", "application/x-lisp");
                _mimeTypes.Add("lst", "text/plain");
                _mimeTypes.Add("lsx", "text/x-la-asf");
                _mimeTypes.Add("ltx", "application/x-latex");
                _mimeTypes.Add("lzh", "application/octet-stream");
                _mimeTypes.Add("lzx", "application/octet-stream");
                _mimeTypes.Add("mid", "audio/mid");
                _mimeTypes.Add("midi", "audio/mid");
                _mimeTypes.Add("mov", "video/quicktime");
                _mimeTypes.Add("movie", "video/x-sgi-movie");
                _mimeTypes.Add("mp2", "video/mpeg");
                _mimeTypes.Add("mp3", "video/mpeg");
                _mimeTypes.Add("mpa", "video/mpeg");
                _mimeTypes.Add("mpe", "video/mpeg");
                _mimeTypes.Add("mpeg", "video/mpeg");
                _mimeTypes.Add("mpg", "video/mpeg");
                _mimeTypes.Add("mpv2", "video/mpeg");
                _mimeTypes.Add("mht", "message/rfc822");
                _mimeTypes.Add("mhtml", "message/rfc822");
                _mimeTypes.Add("m", "text/plain");
                _mimeTypes.Add("m1v", "video/mpeg");
                _mimeTypes.Add("m2a", "audio/mpeg");
                _mimeTypes.Add("m2v", "video/mpeg");
                _mimeTypes.Add("m3u", "audio/x-mpequrl");
                _mimeTypes.Add("man", "application/x-troff-man");
                _mimeTypes.Add("map", "application/x-navimap");
                _mimeTypes.Add("mar", "text/plain");
                _mimeTypes.Add("mbd", "application/mbedlet");
                _mimeTypes.Add("mc$", "application/x-magic-cap-package-1.0");
                _mimeTypes.Add("mcd", "application/mcad");
                _mimeTypes.Add("mcf", "image/vasa");
                _mimeTypes.Add("mcp", "application/netmc");
                _mimeTypes.Add("me", "application/x-troff-me");
                _mimeTypes.Add("mif", "application/x-frame");
                _mimeTypes.Add("mime", "message/rfc822");
                _mimeTypes.Add("mjf", "audio/x-vnd.audioexplosion.mjuicemediafile");
                _mimeTypes.Add("mjpg", "video/x-motion-jpeg");
                _mimeTypes.Add("mm", "application/base64");
                _mimeTypes.Add("mme", "application/base64");
                _mimeTypes.Add("mod", "audio/mod");
                _mimeTypes.Add("moov", "video/quicktime");
                _mimeTypes.Add("mpc", "application/x-project");
                _mimeTypes.Add("mpga", "audio/mpeg");
                _mimeTypes.Add("mpp", "application/vnd.ms-project");
                _mimeTypes.Add("mpt", "application/x-project");
                _mimeTypes.Add("mpv", "application/x-project");
                _mimeTypes.Add("mpx", "application/x-project");
                _mimeTypes.Add("mrc", "application/marc");
                _mimeTypes.Add("ms", "application/x-troff-ms");
                _mimeTypes.Add("mv", "video/x-sgi-movie");
                _mimeTypes.Add("my", "audio/make");
                _mimeTypes.Add("mzz", "application/x-vnd.audioexplosion.mzz");
                _mimeTypes.Add("nap", "image/naplps");
                _mimeTypes.Add("naplps", "image/naplps");
                _mimeTypes.Add("nc", "application/x-netcdf");
                _mimeTypes.Add("ncm", "application/vnd.nokia.configuration-message");
                _mimeTypes.Add("nif", "image/x-niff");
                _mimeTypes.Add("niff", "image/x-niff");
                _mimeTypes.Add("nix", "application/x-mix-transfer");
                _mimeTypes.Add("nsc", "application/x-conference");
                _mimeTypes.Add("nvd", "application/x-navidoc");
                _mimeTypes.Add("o", "application/octet-stream");
                _mimeTypes.Add("oda", "application/oda");
                _mimeTypes.Add("omc", "application/x-omc");
                _mimeTypes.Add("omcd", "application/x-omcdatamaker");
                _mimeTypes.Add("omcr", "application/x-omcregerator");
                _mimeTypes.Add("pdf", "application/pdf");
                _mimeTypes.Add("png", "image/png");
                _mimeTypes.Add("pnz", "image/png");
                _mimeTypes.Add("p12", "application/x-pkcs12");
                _mimeTypes.Add("pfx", "application/x-pkcs12");
                _mimeTypes.Add("p", "text/x-pascal");
                _mimeTypes.Add("p10", "application/x-pkcs10");
                _mimeTypes.Add("p7a", "application/x-pkcs7-signature");
                _mimeTypes.Add("p7c", "application/pkcs7-mime");
                _mimeTypes.Add("p7m", "application/pkcs7-mime");
                _mimeTypes.Add("p7r", "application/x-pkcs7-certreqresp");
                _mimeTypes.Add("p7s", "application/pkcs7-signature");
                _mimeTypes.Add("part", "application/pro_eng");
                _mimeTypes.Add("pas", "text/pascal");
                _mimeTypes.Add("pbm", "image/x-portable-bitmap");
                _mimeTypes.Add("pcl", "application/vnd.hp-pcl");
                _mimeTypes.Add("pct", "image/x-pict");
                _mimeTypes.Add("pcx", "image/x-pcx");
                _mimeTypes.Add("pdb", "chemical/x-pdb");
                _mimeTypes.Add("pfunk", "audio/make");
                _mimeTypes.Add("pgm", "image/x-portable-graymap");
                _mimeTypes.Add("pic", "image/pict");
                _mimeTypes.Add("pict", "image/pict");
                _mimeTypes.Add("pkg", "application/x-newton-compatible-pkg");
                _mimeTypes.Add("pko", "application/vnd.ms-pki.pko");
                _mimeTypes.Add("pl", "text/x-script.perl");
                _mimeTypes.Add("plx", "application/x-pixclscript");
                _mimeTypes.Add("pm", "text/x-script.perl-module");
                _mimeTypes.Add("pm4", "application/x-pagemaker");
                _mimeTypes.Add("pm5", "application/x-pagemaker");
                _mimeTypes.Add("pnm", "image/x-portable-anymap");
                _mimeTypes.Add("pot", "application/vnd.ms-powerpoint");
                _mimeTypes.Add("pov", "model/x-pov");
                _mimeTypes.Add("ppa", "application/vnd.ms-powerpoint");
                _mimeTypes.Add("ppm", "image/x-portable-pixmap");
                _mimeTypes.Add("pps", "application/vnd.ms-powerpoint");
                _mimeTypes.Add("ppt", "application/powerpoint");
                _mimeTypes.Add("ppz", "application/mspowerpoint");
                _mimeTypes.Add("pre", "application/x-freelance");
                _mimeTypes.Add("prt", "application/pro_eng");
                _mimeTypes.Add("ps", "application/postscript");
                _mimeTypes.Add("psd", "application/octet-stream");
                _mimeTypes.Add("pvu", "paleovu/x-pv");
                _mimeTypes.Add("pwz", "application/vnd.ms-powerpoint");
                _mimeTypes.Add("py", "text/x-script.phyton");
                _mimeTypes.Add("pyc", "applicaiton/x-bytecode.python");
                _mimeTypes.Add("qcp", "audio/vnd.qcelp");
                _mimeTypes.Add("qd3", "x-world/x-3dmf");
                _mimeTypes.Add("qd3d", "x-world/x-3dmf");
                _mimeTypes.Add("qif", "image/x-quicktime");
                _mimeTypes.Add("qt", "video/quicktime");
                _mimeTypes.Add("qtc", "video/x-qtc");
                _mimeTypes.Add("qti", "image/x-quicktime");
                _mimeTypes.Add("qtif", "image/x-quicktime");
                _mimeTypes.Add("rss", "application/rss+xml");
                _mimeTypes.Add("rtf", "text/richtext");
                _mimeTypes.Add("rtx", "text/richtext");
                _mimeTypes.Add("ra", "audio/x-realaudio");
                _mimeTypes.Add("ram", "audio/x-pn-realaudio");
                _mimeTypes.Add("ras", "image/x-cmu-raster");
                _mimeTypes.Add("rast", "image/cmu-raster");
                _mimeTypes.Add("rexx", "text/x-script.rexx");
                _mimeTypes.Add("rf", "image/vnd.rn-realflash");
                _mimeTypes.Add("rgb", "image/x-rgb");
                _mimeTypes.Add("rm", "audio/x-pn-realaudio");
                _mimeTypes.Add("rmi", "audio/mid");
                _mimeTypes.Add("rmm", "audio/x-pn-realaudio");
                _mimeTypes.Add("rmp", "audio/x-pn-realaudio");
                _mimeTypes.Add("rng", "application/ringing-tones");
                _mimeTypes.Add("rnx", "application/vnd.rn-realplayer");
                _mimeTypes.Add("roff", "application/x-troff");
                _mimeTypes.Add("rp", "image/vnd.rn-realpix");
                _mimeTypes.Add("rpm", "audio/x-pn-realaudio-plugin");
                _mimeTypes.Add("rt", "text/richtext");
                _mimeTypes.Add("rv", "video/vnd.rn-realvideo");
                _mimeTypes.Add("swf", "application/x-shockwave-flash");
                _mimeTypes.Add("s", "text/x-asm");
                _mimeTypes.Add("s3m", "audio/s3m");
                _mimeTypes.Add("saveme", "application/octet-stream");
                _mimeTypes.Add("sbk", "application/x-tbook");
                _mimeTypes.Add("scm", "video/x-scm");
                _mimeTypes.Add("sdml", "text/plain");
                _mimeTypes.Add("sdp", "application/x-sdp");
                _mimeTypes.Add("sdr", "application/sounder");
                _mimeTypes.Add("sea", "application/x-sea");
                _mimeTypes.Add("set", "application/set");
                _mimeTypes.Add("sgm", "text/x-sgml");
                _mimeTypes.Add("sgml", "text/x-sgml");
                _mimeTypes.Add("sh", "text/x-script.sh");
                _mimeTypes.Add("shar", "application/x-bsh");
                _mimeTypes.Add("shtml", "text/html");
                _mimeTypes.Add("sid", "audio/x-psid");
                _mimeTypes.Add("sit", "application/x-sit");
                _mimeTypes.Add("skd", "application/x-koan");
                _mimeTypes.Add("skm", "application/x-koan");
                _mimeTypes.Add("skp", "application/x-koan");
                _mimeTypes.Add("skt", "application/x-koan");
                _mimeTypes.Add("sl", "application/x-seelogo");
                _mimeTypes.Add("smi", "application/smil");
                _mimeTypes.Add("smil", "application/smil");
                _mimeTypes.Add("snd", "audio/basic");
                _mimeTypes.Add("sol", "application/solids");
                _mimeTypes.Add("spc", "application/x-pkcs7-certificates");
                _mimeTypes.Add("spl", "application/futuresplash");
                _mimeTypes.Add("spr", "application/x-sprite");
                _mimeTypes.Add("sprite", "application/x-sprite");
                _mimeTypes.Add("src", "application/x-wais-source");
                _mimeTypes.Add("ssi", "text/x-server-parsed-html");
                _mimeTypes.Add("ssm", "application/streamingmedia");
                _mimeTypes.Add("sst", "application/vnd.ms-pki.certstore");
                _mimeTypes.Add("step", "application/step");
                _mimeTypes.Add("stl", "application/sla");
                _mimeTypes.Add("stp", "application/step");
                _mimeTypes.Add("sv4cpio", "application/x-sv4cpio");
                _mimeTypes.Add("sv4crc", "application/x-sv4crc");
                _mimeTypes.Add("svf", "image/x-dwg");
                _mimeTypes.Add("svr", "application/x-world");
                _mimeTypes.Add("txt", "text/plain");
                _mimeTypes.Add("tgz", "application/x-compressed");
                _mimeTypes.Add("tif", "image/tiff");
                _mimeTypes.Add("tiff", "image/tiff");
                _mimeTypes.Add("t", "application/x-troff");
                _mimeTypes.Add("talk", "text/x-speech");
                _mimeTypes.Add("tar", "application/x-tar");
                _mimeTypes.Add("tbk", "application/x-tbook");
                _mimeTypes.Add("tcl", "text/x-script.tcl");
                _mimeTypes.Add("tcsh", "text/x-script.tcsh");
                _mimeTypes.Add("tex", "application/x-tex");
                _mimeTypes.Add("texi", "application/x-texinfo");
                _mimeTypes.Add("texinfo", "application/x-texinfo");
                _mimeTypes.Add("text", "text/plain");
                _mimeTypes.Add("tr", "application/x-troff");
                _mimeTypes.Add("tsi", "audio/tsp-audio");
                _mimeTypes.Add("tsp", "audio/tsplayer");
                _mimeTypes.Add("tsv", "text/tab-separated-values");
                _mimeTypes.Add("turbot", "image/florian");
                _mimeTypes.Add("uil", "text/x-uil");
                _mimeTypes.Add("uni", "text/uri-list");
                _mimeTypes.Add("unis", "text/uri-list");
                _mimeTypes.Add("unv", "application/i-deas");
                _mimeTypes.Add("uri", "text/uri-list");
                _mimeTypes.Add("uris", "text/uri-list");
                _mimeTypes.Add("ustar", "application/x-ustar");
                _mimeTypes.Add("uu", "text/x-uuencode");
                _mimeTypes.Add("uue", "text/x-uuencode");
                _mimeTypes.Add("vcd", "application/x-cdlink");
                _mimeTypes.Add("vcs", "text/x-vcalendar");
                _mimeTypes.Add("vda", "application/vda");
                _mimeTypes.Add("vdo", "video/vdo");
                _mimeTypes.Add("vew", "application/groupwise");
                _mimeTypes.Add("viv", "video/vivo");
                _mimeTypes.Add("vivo", "video/vivo");
                _mimeTypes.Add("vmd", "application/vocaltec-media-desc");
                _mimeTypes.Add("vmf", "application/vocaltec-media-file");
                _mimeTypes.Add("voc", "audio/x-voc");
                _mimeTypes.Add("vos", "video/vosaic");
                _mimeTypes.Add("vox", "audio/voxware");
                _mimeTypes.Add("vqe", "audio/x-twinvq-plugin");
                _mimeTypes.Add("vqf", "audio/x-twinvq");
                _mimeTypes.Add("vql", "audio/x-twinvq-plugin");
                _mimeTypes.Add("vrml", "application/x-vrml");
                _mimeTypes.Add("vrt", "x-world/x-vrt");
                _mimeTypes.Add("vsd", "application/x-visio");
                _mimeTypes.Add("vst", "application/x-visio");
                _mimeTypes.Add("vsw", "application/x-visio");
                _mimeTypes.Add("wav", "audio/wav");
                _mimeTypes.Add("wmv", "video/x-ms-wmv");
                _mimeTypes.Add("w60", "application/wordperfect6.0");
                _mimeTypes.Add("w61", "application/wordperfect6.1");
                _mimeTypes.Add("w6w", "application/msword");
                _mimeTypes.Add("wb1", "application/x-qpro");
                _mimeTypes.Add("wbmp", "image/vnd.wap.wbmp");
                _mimeTypes.Add("web", "application/vnd.xara");
                _mimeTypes.Add("wiz", "application/msword");
                _mimeTypes.Add("wk1", "application/x-123");
                _mimeTypes.Add("wmf", "windows/metafile");
                _mimeTypes.Add("wml", "text/vnd.wap.wml");
                _mimeTypes.Add("wmlc", "application/vnd.wap.wmlc");
                _mimeTypes.Add("wmls", "text/vnd.wap.wmlscript");
                _mimeTypes.Add("wmlsc", "application/vnd.wap.wmlscriptc");
                _mimeTypes.Add("word", "application/msword");
                _mimeTypes.Add("wp", "application/wordperfect");
                _mimeTypes.Add("wp5", "application/wordperfect");
                _mimeTypes.Add("wp6", "application/wordperfect");
                _mimeTypes.Add("wpd", "application/wordperfect");
                _mimeTypes.Add("wq1", "application/x-lotus");
                _mimeTypes.Add("wri", "application/mswrite");
                _mimeTypes.Add("wrl", "model/vrml");
                _mimeTypes.Add("wrz", "model/vrml");
                _mimeTypes.Add("wsc", "text/scriplet");
                _mimeTypes.Add("wsrc", "application/x-wais-source");
                _mimeTypes.Add("wtk", "application/x-wintalk");
                _mimeTypes.Add("wsdl", "text/xml");
                _mimeTypes.Add("xml", "text/xml");
                _mimeTypes.Add("xslt", "text/xml");
                _mimeTypes.Add("xsd", "text/xml");
                _mimeTypes.Add("xsf", "text/xml");
                _mimeTypes.Add("xsl", "text/xml");
                _mimeTypes.Add("xlm", "application/vnd.ms-excel");
                _mimeTypes.Add("xls", "application/vnd.ms-excel");
                _mimeTypes.Add("xlsx", "application/vnd.ms-excel");
                _mimeTypes.Add("xlt", "application/vnd.ms-excel");
                _mimeTypes.Add("xbm", "image/xbm");
                _mimeTypes.Add("xdr", "video/x-amt-demorun");
                _mimeTypes.Add("xgz", "xgl/drawing");
                _mimeTypes.Add("xif", "image/vnd.xiff");
                _mimeTypes.Add("xl", "application/excel");
                _mimeTypes.Add("xla", "application/excel");
                _mimeTypes.Add("xlb", "application/excel");
                _mimeTypes.Add("xlc", "application/excel");
                _mimeTypes.Add("xld", "application/excel");
                _mimeTypes.Add("xlk", "application/excel");
                _mimeTypes.Add("xll", "application/excel");
                _mimeTypes.Add("xlv", "application/excel");
                _mimeTypes.Add("xlw", "application/excel");
                _mimeTypes.Add("xm", "audio/xm");
                _mimeTypes.Add("xmz", "xgl/movie");
                _mimeTypes.Add("xpix", "application/x-vnd.ls-xpix");
                _mimeTypes.Add("xpm", "image/x-xpixmap");
                _mimeTypes.Add("x-png", "image/png");
                _mimeTypes.Add("xsr", "video/x-amt-showrun");
                _mimeTypes.Add("xwd", "image/x-xwd");
                _mimeTypes.Add("xyz", "chemical/x-pdb");
                _mimeTypes.Add("z", "application/x-compress");
                _mimeTypes.Add("zip", "application/x-zip-compressed");
                _mimeTypes.Add("zoo", "application/octet-stream");
                _mimeTypes.Add("zsh", "text/x-script.zsh");
            }

            IEnumerable<KeyValuePair<string, string>> mimeTypes = _mimeTypes.Where(u => u.Key.ToLower() == extension.Trim(new char[] { '.' }).ToLower());
            if (mimeTypes.Count() > 0)
                return mimeTypes.First().Value;
            else
                return "application/plain";
        }
    }
}
