/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openpgp
{
    /// <summary>
    /// Open pgp certificate identity container.
    /// </summary>
    public class Identity
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the certificate.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="comments">Any comments for the certificate.</param>
        public Identity(string name, string emailAddress, string comments = "")
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException("emailAddress");

            // Assign each subject varibale.
            _name = name;
            _emailAddress = emailAddress;
            _comments = comments;

            // Assign request questions.
            _nameTextEndWith = _subjectItems[0];
            _emailAddressTextEndWith = _subjectItems[1];
            _commentsTextEndWith = _subjectItems[2];
        }

        private string _name = string.Empty;
        private string _comments = string.Empty;
        private string _emailAddress = string.Empty;

        private string[] _subjectItems = new string[]
        {
            "Name (Name of certificate)",
            "Email address:",
            "Comments"
        };

        private string _nameTextEndWith = string.Empty;
        private string _emailAddressTextEndWith = string.Empty;
        private string _commentsTextEndWith = string.Empty;

        /// <summary>
        /// Gets the certificate name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the certificate email address.
        /// </summary>
        public string EmailAddress
        {
            get { return _emailAddress; }
        }

        /// <summary>
        /// Gets the certificate comments.
        /// </summary>
        public string Comments
        {
            get { return _comments; }
        }

        /// <summary>
        /// Gets sets the name request.
        /// </summary>
        protected virtual string NameRequest
        {
            get { return _nameTextEndWith; }
            set
            {
                _nameTextEndWith = value;
                _subjectItems[0] = _nameTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the email address request.
        /// </summary>
        protected virtual string EmailAddressRequest
        {
            get { return _emailAddressTextEndWith; }
            set
            {
                _emailAddressTextEndWith = value;
                _subjectItems[1] = _emailAddressTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the comments request.
        /// </summary>
        protected virtual string CommentsRequest
        {
            get { return _commentsTextEndWith; }
            set
            {
                _commentsTextEndWith = value;
                _subjectItems[2] = _commentsTextEndWith;
            }
        }

        /// <summary>
        /// Creates the identity list from the identity arguments.
        /// </summary>
        /// <returns>The well formed identity</returns>
        public virtual string IdentityArguments()
        {
            return _name + (String.IsNullOrEmpty(_comments) ? " " : " (" + _comments + ") ") + "<" + _emailAddress + ">";
        }

        /// <summary>
        /// Creates the identity list from the identity arguments.
        /// </summary>
        /// <returns>Returns a string that represents the current object.</returns>
        public override string ToString()
        {
            return IdentityArguments();
        }

        /// <summary>
        /// Gets the response subject value for the request.
        /// </summary>
        /// <param name="request">The request for the subject argument</param>
        /// <returns>The well formed response from the request.</returns>
        public virtual string Response(string request)
        {
            if (String.IsNullOrEmpty(request)) throw new ArgumentNullException("request");

            // Find the request from the list for the current request.
            IEnumerable<string> requestItem = _subjectItems.Where(u => request.ToLower().EndsWith(u.ToLower()));

            // If request has been found
            if (requestItem != null)
            {
                // If request has been found
                if (requestItem.Count() > 0)
                {
                    // Attempt to find each response for the current request.
                    if (_nameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return Name;
                    else if (_emailAddressTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return EmailAddress;
                    else if (_commentsTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return Comments;
                    else
                        return "";
                }
                else
                    return "";
            }
            else
                return "";
        }
    }
}
