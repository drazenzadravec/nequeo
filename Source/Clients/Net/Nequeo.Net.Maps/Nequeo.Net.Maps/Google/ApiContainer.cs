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

namespace Nequeo.Net.Maps.Google
{
    /// <summary>
    /// Google api map container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Google api map container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        public ApiContainer(Uri serviceRoot, string apiKey)
        {
            _apiKey = apiKey;
            _serviceRoot = serviceRoot;
        }

        private string _apiKey = null;
        private Uri _serviceRoot = null;

        /// <summary>
        /// Get the current time zone for the map location.
        /// </summary>
        /// <param name="longitude">The longitude (vertical) value from GMT.</param>
        /// <param name="latitude">The latitude (horizontal) value from GMT.</param>
        /// <param name="timestamp">The current time stamp. Specifies the desired time 
        /// as seconds since midnight, January 1, 1970 UTC. The Google Maps Time Zone API 
        /// uses the timestamp to determine whether or not Daylight Savings should be 
        /// applied. Times before 1970 can be expressed as negative values.</param>
        /// <returns>The time zone.</returns>
        public TimeZone GetTimeZone(double longitude, double latitude, long timestamp)
        {
            string query = "";

            query += "&location=" + latitude.ToString() + "," + longitude.ToString() + "&timestamp=" + timestamp.ToString();

            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessJsonRequest<TimeZone>(query);
        }

        /// <summary>
        /// Get the geocoding information for the address.
        /// </summary>
        /// <param name="address">The address to get geocoding for.</param>
        /// <returns>The geocoding address.</returns>
        public Geocoding GetGeocode(string address)
        {
            string query = "";

            query += "&address=" + System.Uri.EscapeDataString(address);

            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessJsonRequest<Geocoding>(query);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="query">The query to apply.</param>
        /// <param name="serviceName">The service name to call.</param>
        /// <returns>The returned type.</returns>
        private T ProcessJsonRequest<T>(string query, string serviceName = "json")
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + serviceName + "?" + query.TrimStart(new char[] { '&' }));
            return Nequeo.Net.HttpJsonClient.Request<T>(constructedServiceUri);
        }
    }
}
