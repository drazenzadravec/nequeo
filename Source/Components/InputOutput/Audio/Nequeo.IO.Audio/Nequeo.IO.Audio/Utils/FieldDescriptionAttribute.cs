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

namespace Nequeo.IO.Audio.Utils
{
    /// <summary>
    /// Allows us to add descriptions to interop members
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class FieldDescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Field description
        /// </summary>
        public FieldDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
