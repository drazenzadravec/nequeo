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
using System.Net;
using System.Threading.Tasks;

namespace Nequeo.Net.Maps.Microsoft
{
    /// <summary>
    /// Microsoft map geocoding apis.
    /// </summary>
    public sealed class GeocodingApi
    {
        /// <summary>
        /// Microsoft map geocoding apis.
        /// </summary>
        public GeocodingApi()
        {
            // Gets the google maps service URI.
            _service = new Uri(Nequeo.Net.Maps.Properties.Settings.Default.MicrosoftMapsGeocodeServiceURI);
        }

        /// <summary>
        /// Microsoft map geocoding apis.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public GeocodingApi(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private NetworkCredential _credentials = null;
        private ApiContainer _container = null;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the api key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Get the geocoding information for the address.
        /// </summary>
        /// <param name="address">The address to get geocoding for.</param>
        /// <returns>The geocoding address.</returns>
        public Geocoding GetGeocode(string address)
        {
            Initialise();
            return _container.GetGeocode(address);
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // If the container needs to be translated.
            if (_container == null)
            {
                // Create the container.
                _container = new ApiContainer(_service, _credentials.UserName);
            }
        }
    }
}
