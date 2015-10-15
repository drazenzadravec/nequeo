/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Nequeo.Net.OAuth;
using Nequeo.Net.OAuth.Framework;

namespace Nequeo.Net.OAuth.Client.Version1
{
    /// <summary>
    /// The many specific authorization scopes Google offers.
    /// </summary>
    [Flags]
    public enum GoogleApplications : long
    {
        /// <summary>
        /// The Gmail address book.
        /// </summary>
        Contacts = 0x1,

        /// <summary>
        /// Appointments in Google Calendar.
        /// </summary>
        Calendar = 0x2,

        /// <summary>
        /// Blog post authoring.
        /// </summary>
        Blogger = 0x4,

        /// <summary>
        /// Google Finance
        /// </summary>
        Finance = 0x8,

        /// <summary>
        /// Google Gmail
        /// </summary>
        Gmail = 0x10,

        /// <summary>
        /// Google Health
        /// </summary>
        Health = 0x20,

        /// <summary>
        /// Google OpenSocial
        /// </summary>
        OpenSocial = 0x40,

        /// <summary>
        /// Picasa Web
        /// </summary>
        PicasaWeb = 0x80,

        /// <summary>
        /// Google Spreadsheets
        /// </summary>
        Spreadsheets = 0x100,

        /// <summary>
        /// Webmaster Tools
        /// </summary>
        WebmasterTools = 0x200,

        /// <summary>
        /// YouTube service
        /// </summary>
        YouTube = 0x400,

        /// <summary>
        /// Google Docs
        /// </summary>
        DocumentsList = 0x800,

        /// <summary>
        /// Google Book Search
        /// </summary>
        BookSearch = 0x1000,

        /// <summary>
        /// Google Base
        /// </summary>
        GoogleBase = 0x2000,

        /// <summary>
        /// Google Analytics
        /// </summary>
        Analytics = 0x4000,

        /// <summary>
        /// Google Maps
        /// </summary>
        Maps = 0x8000,
    }
}
