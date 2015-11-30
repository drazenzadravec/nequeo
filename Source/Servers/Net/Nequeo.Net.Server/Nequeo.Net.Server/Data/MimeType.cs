using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Data
{
    /// <summary>
    /// Mime types.
    /// </summary>
    public class MimeType
    {
        /// <summary>
        /// Is the mime type an application hosting type.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>True if application hosting type; else false.</returns>
        public static bool IsApplicationType(string extension)
        {
            return Nequeo.Net.Mime.MimeType.IsApplicationType(extension);
        }

        /// <summary>
        /// Get the mime type for the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The mime type.</returns>
        public static string GetMimeType(string extension)
        {
            return Nequeo.Net.Mime.MimeType.GetMimeType(extension);
        }
    }
}
