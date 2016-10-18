/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

using Nequeo.Collections;

namespace Nequeo.Net.Http2.Protocol
{
    /// <summary>
    /// HTTP/2 Version Identification.
    /// </summary>
    public static class Protocols
    {
        /// <summary>
        /// Http /2.
        /// </summary>
        public static string Http2 = "h2-12";
        /// <summary>
        /// Http /2 no TLS.
        /// </summary>
        public static string Http2NoTls = "h2c-12";
        /// <summary>
        /// Http /1.1
        /// </summary>
        public static string Http1 = "http/1.1";
    }

    /// <summary>
    /// This class contains the most commonly used constants.
    /// </summary>
    internal static class Constants
    {
        /* 12 -> 4.1 
        All frames begin with an 9-octet header followed by a payload of
        between 0 and 16,383 octets. */
        public const int FramePreambleSize = 9; // bytes
        public const int DefaultClientCertVectorSize = 9;

        /* 12 -> 4.2 
        The absolute maximum size of a frame payload is 2^14-1 (16,383) octets,
        meaning that the maximum frame size is 16,391 octets. */
        public const int MaxFramePayloadSize = 0x3fff; // 16383 bytes.
        public const int MaxFramePaddingSize = 300; // bytes
        public const int InitialFlowControlOptionsValue = 0;
        public const string DefaultMethod = "GET";
        public const string DefaultHost = "localhost";
        /* 12 -> 6.5.2 
        It is recommended that this value be no smaller than 100, so as to not
        unnecessarily limit parallelism. */
        public const int DefaultMaxConcurrentStreams = 100;
        /* 12 -> 6.9.1
        A sender MUST NOT allow a flow control window to exceed 2^31 - 1 bytes. */
        public const int MaxWindowSize = 0x7FFFFFFF;
        public const int MaxPriority = 0x7fffffff;

        /* 12 -> 6.9.2
        When a HTTP/2 connection is first established, new streams are
        created with an initial flow control window size of 65535 bytes.
        The connection flow control window is 65535 bytes. */
        public const int InitialFlowControlWindowSize = 0xFFFF;
        public const int DefaultStreamPriority = 1 << 30;

        /* 12 -> 5.3.5
        Streams are assigned a dependency on stream 0x0. Pushed streams initially
        depend on their associated stream. In both cases, streams are assigned a 
        default weight of 16. */
        public const int DefaultStreamDependency = 0;
        public const int DefaultStreamWeight = 16;


        /* HPACK see spec 7 -> Appendix B.  Static Table
          +-------+-----------------------------+--------------+
          | Index | Header Name                 | Header Value |
          +-------+-----------------------------+--------------+
          | 1     | :authority                  |              |
          | 2     | :method                     | GET          |
          | 3     | :method                     | POST         |
          | 4     | :path                       | /            |
          | 5     | :path                       | /index.html  |
          | 6     | :scheme                     | http         |
          | 7     | :scheme                     | https        |
          | 8     | :status                     | 200          |
          | 9     | :status                     | 204          |
          | 10    | :status                     | 206          |
          | 11    | :status                     | 304          |
          | 12    | :status                     | 400          |
          | 13    | :status                     | 404          |
          | 14    | :status                     | 500          |
          | 15    | accept-charset              |              |
          | 16    | accept-encoding             | gzip, deflate|
          | 17    | accept-language             |              |
          | 18    | accept-ranges               |              |
          | 19    | accept                      |              |
          | 20    | access-control-allow-origin |              |
          | 21    | age                         |              |
          | 22    | allow                       |              |
          | 23    | authorization               |              |
          | 24    | cache-control               |              |
          | 25    | content-disposition         |              |
          | 26    | content-encoding            |              |
          | 27    | content-language            |              |
          | 28    | content-length              |              |
          | 29    | content-location            |              |
          | 30    | content-range               |              |
          | 31    | content-type                |              |
          | 32    | cookie                      |              |
          | 33    | date                        |              |
          | 34    | etag                        |              |
          | 35    | expect                      |              |
          | 36    | expires                     |              |
          | 37    | from                        |              |
          | 38    | host                        |              |
          | 39    | if-match                    |              |
          | 40    | if-modified-since           |              |
          | 41    | if-none-match               |              |
          | 42    | if-range                    |              |
          | 43    | if-unmodified-since         |              |
          | 44    | last-modified               |              |
          | 45    | link                        |              |
          | 46    | location                    |              |
          | 47    | max-forwards                |              |
          | 48    | proxy-authenticate          |              |
          | 49    | proxy-authorization         |              |
          | 50    | range                       |              |
          | 51    | referer                     |              |
          | 52    | refresh                     |              |
          | 53    | retry-after                 |              |
          | 54    | server                      |              |
          | 55    | set-cookie                  |              |
          | 56    | strict-transport-security   |              |
          | 57    | transfer-encoding           |              |
          | 58    | user-agent                  |              |
          | 59    | vary                        |              |
          | 60    | via                         |              |
          | 61    | www-authenticate            |              |
          +-------+-----------------------------+--------------+ */

        /// <summary>
        /// Get the static header list table.
        /// </summary>
        public static readonly HeadersList StaticTable = new HeadersList(new[]
            {
                    new KeyValuePair<string, string>(":authority", String.Empty),                               //1
                    new KeyValuePair<string, string>(":method", "GET"),                                         //2
                    new KeyValuePair<string, string>(":method", "POST"),                                        //3
                    new KeyValuePair<string, string>(":path", "/"),                                             //4
                    new KeyValuePair<string, string>(":path", "/index.html"),                                   //5
                    new KeyValuePair<string, string>(":scheme", "http"),                                        //6
                    new KeyValuePair<string, string>(":scheme", "https"),                                       //7
                    new KeyValuePair<string, string>(":status", "200"),                                         //8
                    new KeyValuePair<string, string>(":status", "204"),                                         //9
                    new KeyValuePair<string, string>(":status", "206"),                                         //10
                    new KeyValuePair<string, string>(":status", "304"),                                         //11
                    new KeyValuePair<string, string>(":status", "400"),                                         //12
                    new KeyValuePair<string, string>(":status", "404"),                                         //13
                    new KeyValuePair<string, string>(":status", "500"),                                         //14
                    new KeyValuePair<string, string>("accept-charset", String.Empty),                           //15
                    new KeyValuePair<string, string>("accept-encoding", "gzip, deflate"),                       //16
                    new KeyValuePair<string, string>("accept-language", String.Empty),                          //17
                    new KeyValuePair<string, string>("accept-ranges", String.Empty),                            //18
                    new KeyValuePair<string, string>("accept", String.Empty),                                   //19
                    new KeyValuePair<string, string>("access-control-allow-origin", String.Empty),              //20
                    new KeyValuePair<string, string>("age", String.Empty),                                      //21
                    new KeyValuePair<string, string>("allow", String.Empty),                                    //22
                    new KeyValuePair<string, string>("authorization", String.Empty),                            //23
                    new KeyValuePair<string, string>("cache-control", String.Empty),                            //24
                    new KeyValuePair<string, string>("content-disposition", String.Empty),                      //25
                    new KeyValuePair<string, string>("content-encoding", String.Empty),                         //26
                    new KeyValuePair<string, string>("content-language", String.Empty),                         //27
                    new KeyValuePair<string, string>("content-length", String.Empty),                           //28
                    new KeyValuePair<string, string>("content-location", String.Empty),                         //29
                    new KeyValuePair<string, string>("content-range", String.Empty),                            //30
                    new KeyValuePair<string, string>("content-type", String.Empty),                             //31   
                    new KeyValuePair<string, string>("cookie", String.Empty),                                   //32
                    new KeyValuePair<string, string>("date", String.Empty),                                     //33
                    new KeyValuePair<string, string>("etag", String.Empty),                                     //34   
                    new KeyValuePair<string, string>("expect", String.Empty),                                   //35
                    new KeyValuePair<string, string>("expires", String.Empty),                                  //36
                    new KeyValuePair<string, string>("from", String.Empty),                                     //37
                    new KeyValuePair<string, string>("host", String.Empty),                                     //38
                    new KeyValuePair<string, string>("if-match", String.Empty),                                 //39
                    new KeyValuePair<string, string>("if-modified-since", String.Empty),                        //40
                    new KeyValuePair<string, string>("if-none-match", String.Empty),                            //41
                    new KeyValuePair<string, string>("if-range", String.Empty),                                 //42 
                    new KeyValuePair<string, string>("if-unmodified-since", String.Empty),                      //43
                    new KeyValuePair<string, string>("last-modified", String.Empty),                            //44
                    new KeyValuePair<string, string>("link", String.Empty),                                     //45
                    new KeyValuePair<string, string>("location", String.Empty),                                 //46
                    new KeyValuePair<string, string>("max-forwards", String.Empty),                             //47
                    new KeyValuePair<string, string>("proxy-authenticate", String.Empty),                       //48
                    new KeyValuePair<string, string>("proxy-authorization", String.Empty),                      //49
                    new KeyValuePair<string, string>("range", String.Empty),                                    //50
                    new KeyValuePair<string, string>("referer", String.Empty),                                  //51
                    new KeyValuePair<string, string>("refresh", String.Empty),                                  //52
                    new KeyValuePair<string, string>("retry-after", String.Empty),                              //53
                    new KeyValuePair<string, string>("server", String.Empty),                                   //54
                    new KeyValuePair<string, string>("set-cookie", String.Empty),                               //55
                    new KeyValuePair<string, string>("strict-transport-security", String.Empty),                //56
                    new KeyValuePair<string, string>("transfer-encoding", String.Empty),                        //57
                    new KeyValuePair<string, string>("user-agent", String.Empty),                               //58
                    new KeyValuePair<string, string>("vary", String.Empty),                                     //59
                    new KeyValuePair<string, string>("via", String.Empty),                                      //60
                    new KeyValuePair<string, string>("www-authenticate", String.Empty),                         //61
            });
    }
}
