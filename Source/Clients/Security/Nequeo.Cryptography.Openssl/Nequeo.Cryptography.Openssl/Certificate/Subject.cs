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
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openssl
{
    /// <summary>
    /// Open ssl certificate subject container
    /// </summary>
    public class Subject
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="countryName">The country name (code)</param>
        /// <param name="state">The country state</param>
        /// <param name="locationName">The city location</param>
        /// <param name="organisationName">The organisation name (company name)</param>
        /// <param name="organisationUnitName">The orgination unit name (department)</param>
        /// <param name="commonName">The common name (e.g. www.company.com, Alice Smith</param>
        /// <param name="emailAddress">The authority email adress.</param>
        public Subject(string countryName, string state, string locationName, string organisationName,
            string organisationUnitName, string commonName, string emailAddress = null)
        {
            if (String.IsNullOrEmpty(countryName)) throw new ArgumentNullException(nameof(countryName));
            if (String.IsNullOrEmpty(state)) throw new ArgumentNullException(nameof(state));
            if (String.IsNullOrEmpty(locationName)) throw new ArgumentNullException(nameof(locationName));
            if (String.IsNullOrEmpty(organisationName)) throw new ArgumentNullException(nameof(organisationName));
            if (String.IsNullOrEmpty(organisationUnitName)) throw new ArgumentNullException(nameof(organisationUnitName));
            if (String.IsNullOrEmpty(commonName)) throw new ArgumentNullException(nameof(commonName));

            // Assign each subject varibale.
            _countryName = countryName;
            _state = state;
            _locationName = locationName;
            _organisationName = organisationName;
            _organisationUnitName = organisationUnitName;
            _commonName = commonName;
            _emailAddress = emailAddress;

            // Assign request questions.
            _countryNameTextEndWith = _subjectItems[0];
            _stateTextEndWith = _subjectItems[1];
            _locationNameTextEndWith = _subjectItems[2];
            _organisationNameTextEndWith = _subjectItems[3];
            _organisationUnitNameTextEndWith = _subjectItems[4];
            _commonNameTextEndWith = _subjectItems[5];
            _emailAddressTextEndWith = _subjectItems[6];
        }

        private string _countryName = string.Empty;
        private string _state = string.Empty;
        private string _locationName = string.Empty;
        private string _organisationName = string.Empty;
        private string _organisationUnitName = string.Empty;
        private string _commonName = string.Empty;
        private string _emailAddress = string.Empty;

        private string[] _subjectItems = new string[]
        {
            "country name (2 letter code) [au]:",
            "state or province name (full name) [some-state]:",
            "locality name (eg, city) []:",
            "organization name (eg, company) [nequeo pty ltd]:",
            "organizational unit name (eg, section) []:",
            "common name (eg, your name) []:",
            "email address []:"
        };

        private string _countryNameTextEndWith = string.Empty;
        private string _stateTextEndWith = string.Empty;
        private string _locationNameTextEndWith = string.Empty;
        private string _organisationNameTextEndWith = string.Empty;
        private string _organisationUnitNameTextEndWith = string.Empty;
        private string _commonNameTextEndWith = string.Empty;
        private string _emailAddressTextEndWith = string.Empty;

        /// <summary>
        /// Gets the country name.
        /// </summary>
        public string CountryName
        {
            get { return _countryName; }
        }

        /// <summary>
        /// Gets the state
        /// </summary>
        public string State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets the location name
        /// </summary>
        public string LocationName
        {
            get { return _locationName; }
        }

        /// <summary>
        /// Gets the organisation name
        /// </summary>
        public string OrganisationName
        {
            get { return _organisationName; }
        }

        /// <summary>
        /// Gets the organisation unit name
        /// </summary>
        public string OrganisationUnitName
        {
            get { return _organisationUnitName; }
        }

        /// <summary>
        /// Gets the comman name
        /// </summary>
        public string CommonName
        {
            get { return _commonName; }
        }

        /// <summary>
        /// Gets the email address
        /// </summary>
        public string EmailAddress
        {
            get { return _emailAddress; }
        }

        /// <summary>
        /// Gets sets the country name request.
        /// </summary>
        protected virtual string CountryNameRequest
        {
            get { return _countryNameTextEndWith; }
            set
            {
                _countryNameTextEndWith = value;
                _subjectItems[0] = _countryNameTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the state request.
        /// </summary>
        protected virtual string StateRequest
        {
            get { return _stateTextEndWith; }
            set
            {
                _stateTextEndWith = value;
                _subjectItems[1] = _stateTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the location name request.
        /// </summary>
        protected virtual string LocationNameRequest
        {
            get { return _locationNameTextEndWith; }
            set
            {
                _locationNameTextEndWith = value;
                _subjectItems[2] = _locationNameTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the organisation name request.
        /// </summary>
        protected virtual string OrganisationNameRequest
        {
            get { return _organisationNameTextEndWith; }
            set
            {
                _organisationNameTextEndWith = value;
                _subjectItems[3] = _organisationNameTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the organisation unit name request.
        /// </summary>
        protected virtual string OrganisationUnitNameRequest
        {
            get { return _organisationUnitNameTextEndWith; }
            set
            {
                _organisationUnitNameTextEndWith = value;
                _subjectItems[4] = _organisationUnitNameTextEndWith;
            }
        }

        /// <summary>
        /// Gets sets the common name request.
        /// </summary>
        protected virtual string CommonNameRequest
        {
            get { return _commonNameTextEndWith; }
            set
            {
                _commonNameTextEndWith = value;
                _subjectItems[5] = _commonNameTextEndWith;
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
                _subjectItems[6] = _emailAddressTextEndWith;
            }
        }

        /// <summary>
        /// Creates the subject list from the subject arguments.
        /// </summary>
        /// <returns>The well formed subject</returns>
        public virtual string SubjectArguments()
        {
            return
                "/C=" + _countryName +
                "/ST=" + _state +
                "/L=" + _locationName +
                "/O=" + _organisationName +
                "/OU=" + _organisationUnitName +
                "/CN=" + _commonName +
                (String.IsNullOrEmpty(_emailAddress) ? "" : "/emailAddress=" + _emailAddress);
        }

        /// <summary>
        /// Creates the subject list from the subject arguments.
        /// </summary>
        /// <returns>Returns a string that represents the current object.</returns>
        public override string ToString()
        {
            return SubjectArguments();
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
                    if (_countryNameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return CountryName;
                    else if (_stateTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return State;
                    else if (_locationNameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return LocationName;
                    else if (_organisationNameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return OrganisationName;
                    else if (_organisationUnitNameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return OrganisationUnitName;
                    else if (_commonNameTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return CommonName;
                    else if (_emailAddressTextEndWith.ToLower().Contains(requestItem.First().ToLower()))
                        return EmailAddress;
                    else
                        return "";
                }
                else
                    return "";
            }
            else
                return "";
        }

        /// <summary>
        /// Get the Nequeo Pty Ltd certificate authority issuer DN.
        /// </summary>
        /// <returns>The issuer DN.</returns>
        public static Subject GetNequeoPtyLtdCA()
        {
            return new Subject("AU", "NSW", "Sydney", "Nequeo Pty Ltd", "Development", "Nequeo Root CA", "admin@nequeo.net.au");
        }
    }
}

