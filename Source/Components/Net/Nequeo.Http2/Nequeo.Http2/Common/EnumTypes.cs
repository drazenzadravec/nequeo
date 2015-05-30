/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Unsigned var int prefix.
    /// </summary>
    internal enum UVarIntPrefix : byte
    {
        /// <summary>
        /// Without indexing.
        /// </summary>
        WithoutIndexing = 4,
        /// <summary>
        /// Never index.
        /// </summary>
        NeverIndexed = 4,
        /// <summary>
        /// Encoding context update.
        /// </summary>
        EncodingContextUpdate = 4,
        /// <summary>
        /// Incremental.
        /// </summary>
        Incremental = 6,
        /// <summary>
        /// Indexed.
        /// </summary>
        Indexed = 7,
    }

    /// <summary>
    /// Indexation type.
    /// </summary>
    internal enum IndexationType : byte
    {
        // See spec 07 -> 4.3.2.  Literal Header Field without Indexing
        // The literal header field without indexing representation starts with
        // the '0000' 4-bit pattern.

        /// <summary>
        /// Without indeaxtion.
        /// </summary>
        WithoutIndexation = 0x00,    //07: Literal without indexation            | 0 | 0 | 0 | 0 | Index (4+)       |

        // See spec 07 -> 4.3.3.  Literal Header Field never Indexed
        // The literal header field never indexed representation starts with the
        // '0001' 4-bit pattern.

        /// <summary>
        /// Nerver indexed.
        /// </summary>
        NeverIndexed = 0x10,         //07: Literal Never Indexed                 | 0 | 0 | 0 | 1 | Index (4+)       |

        // See spec 07 -> 4.4.  Encoding Context Update
        // An encoding context update starts with the '001' 3-bit pattern.

        /// <summary>
        /// Encoding context update.
        /// </summary>
        EncodingContextUpdate = 0x20, //07:Encoding Context Update                | 0 | 0 | 1 |    Index (4+)       |

        // See spec 07 -> 4.3.1.  Literal Header Field with Incremental Indexing
        // This representation starts with the '01' 2-bit pattern.

        /// <summary>
        /// Incremental.
        /// </summary>
        Incremental = 0x40,          //07: Literal with incremental indexing     | 0 | 1 |         Index (6+)       |

        /// <summary>
        /// Indexed.
        /// </summary>
        Indexed = 0x80,               //07: Indexed                                | 1 |            Index (7+)       |
    }

    /// <summary>
    /// Common headers.
    /// </summary>
    internal static class CommonHeaders
    {
        /// <summary>
        /// Version
        /// </summary>
        public const string Version = ":version";

        /// <summary>
        /// Status
        /// </summary>
        public const string Status = ":status";

        /// <summary>
        /// Path
        /// </summary>
        public const string Path = ":path";

        /// <summary>
        /// Method
        /// </summary>
        public const string Method = ":method";

        /// <summary>
        /// Max Concurrent Streams
        /// </summary>
        public const string MaxConcurrentStreams = ":max_concurrent_streams";

        /// <summary>
        /// Scheme
        /// </summary>
        public const string Scheme = ":scheme";

        /// <summary>
        /// Initia lWindow Size
        /// </summary>
        public const string InitialWindowSize = ":initial_window_size";

        /// <summary>
        /// Authority
        /// </summary>
        public const string Authority = ":authority";

        /// <summary>
        /// Host
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// Http2 Settings
        /// </summary>
        public const string Http2Settings = "Http2-Settings";

        /// <summary>
        /// Connection
        /// </summary>
        public const string Connection = "Connection";

        /// <summary>
        /// Upgrade
        /// </summary>
        public const string Upgrade = "Upgrade";

        /// <summary>
        /// Content Length
        /// </summary>
        public const string ContentLength = "content-length";

        /// <summary>
        /// Cookie
        /// </summary>
        public const string Cookie = "cookie";
    }
}
