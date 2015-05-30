/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Foundation
{
    /// <summary>
    /// Contains media type information for registering a Media Foundation transform (MFT). 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class MFT_REGISTER_TYPE_INFO
    {
        /// <summary>
        /// The major media type.
        /// </summary>
        public Guid guidMajorType;
        /// <summary>
        /// The Media Subtype
        /// </summary>
        public Guid guidSubtype;
    }
}
