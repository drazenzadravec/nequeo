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

        /// <summary>
        /// Google api map container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        /// <param name="signingSecret">The api signing secret used to access this service.</param>
        public ApiContainer(Uri serviceRoot, string apiKey, string signingSecret)
        {
            _apiKey = apiKey;
            _signingSecret = signingSecret;
            _serviceRoot = serviceRoot;
        }

        private string _signingSecret = null;
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
        /// Get the static image map.
        /// </summary>
        /// <param name="maps">The static map model.</param>
        /// <returns>The array of bytes containing image data.</returns>
        public byte[] GetStaticMapImage(MapsStatic maps)
        {
            string query = "";

            if (maps.Format != null)
            {
                query += "&format=" + System.Uri.EscapeDataString(maps.Format);
            }
            if (maps.Language != null)
            {
                query += "&language=" + System.Uri.EscapeDataString(maps.Language);
            }
            if (maps.MapType != null)
            {
                query += "&maptype=" + System.Uri.EscapeDataString(maps.MapType);
            }
            if (maps.Region != null)
            {
                query += "&region=" + System.Uri.EscapeDataString(maps.Region);
            }
            if (maps.Scale != null)
            {
                query += "&scale=" + System.Uri.EscapeDataString(maps.Scale);
            }
            if (maps.Size != null)
            {
                query += "&size=" + System.Uri.EscapeDataString(maps.Size);
            }
            if (maps.Feature != null)
            {
                if (maps.Feature.Path != null)
                {
                    string pathQuery = "";

                    if (maps.Feature.Path.Color != null)
                    {
                        pathQuery += "color:" + maps.Feature.Path.Color + "|";
                    }
                    if (maps.Feature.Path.FillColor != null)
                    {
                        pathQuery += "fillcolor:" + maps.Feature.Path.FillColor + "|";
                    }
                    if (maps.Feature.Path.Geodesic != null)
                    {
                        pathQuery += "geodesic:" + maps.Feature.Path.Geodesic + "|";
                    }
                    if (maps.Feature.Path.Weight != null)
                    {
                        pathQuery += "weight:" + maps.Feature.Path.Weight + "|";
                    }
                    if (maps.Feature.Path.Points != null)
                    {
                        foreach (string point in maps.Feature.Path.Points)
                        {
                            pathQuery += point + "|";
                        }
                    }
                    if (!String.IsNullOrEmpty(pathQuery))
                    {
                        query += "&path=" + System.Uri.EscapeDataString(pathQuery.TrimEnd('|'));
                    }
                }
                if (maps.Feature.Styles != null)
                {
                    foreach (MapsStaticFeature.MapStyle style in maps.Feature.Styles)
                    {
                        string styleQuery = "";

                        if (style.Color != null)
                        {
                            styleQuery += "color:" + style.Color + "|";
                        }
                        if (style.Element != null)
                        {
                            styleQuery += "element:" + style.Element + "|";
                        }
                        if (style.Feature != null)
                        {
                            styleQuery += "feature:" + style.Feature + "|";
                        }
                        if (style.FillColor != null)
                        {
                            styleQuery += "fillcolor:" + style.FillColor + "|";
                        }
                        if (style.Gamma != null)
                        {
                            styleQuery += "gamma:" + style.Gamma + "|";
                        }
                        if (style.Hue != null)
                        {
                            styleQuery += "hue:" + style.Hue + "|";
                        }
                        if (style.InverseLightness != null)
                        {
                            styleQuery += "inverse_lightness:" + style.InverseLightness + "|";
                        }
                        if (style.Lightness != null)
                        {
                            styleQuery += "lightness:" + style.Lightness + "|";
                        }
                        if (style.Saturation != null)
                        {
                            styleQuery += "saturation:" + style.Saturation + "|";
                        }
                        if (style.Visibility != null)
                        {
                            styleQuery += "visibility:" + style.Visibility + "|";
                        }
                        if (style.Weight != null)
                        {
                            styleQuery += "weight:" + style.Weight + "|";
                        }
                        if (!String.IsNullOrEmpty(styleQuery))
                        {
                            query += "&style=" + System.Uri.EscapeDataString(styleQuery.TrimEnd('|'));
                        }
                    }
                }
                if (maps.Feature.Visible != null)
                {
                    query += "&visible=" + System.Uri.EscapeDataString(maps.Feature.Visible);
                }
                if (maps.Feature.Markers != null)
                {
                    foreach (MapsStaticFeature.MapMarker marker in maps.Feature.Markers)
                    {
                        string markerQuery = "";

                        if (marker.Color != null)
                        {
                            markerQuery += "color:" + marker.Color + "|";
                        }
                        if (marker.Label != null)
                        {
                            markerQuery += "label:" + marker.Label + "|";
                        }
                        if (marker.Size != null)
                        {
                            markerQuery += "size:" + marker.Size + "|";
                        }
                        if (marker.Points != null)
                        {
                            foreach (string point in marker.Points)
                            {
                                markerQuery += point + "|";
                            }
                        }
                        if (!String.IsNullOrEmpty(markerQuery))
                        {
                            query += "&markers=" + System.Uri.EscapeDataString(markerQuery.TrimEnd('|'));
                        }
                    }
                }
            }
            if (maps.Location != null)
            {
                if (maps.Location.Center != null)
                {
                    query += "&center=" + System.Uri.EscapeDataString(maps.Location.Center);
                }
                if (maps.Location.Zoom != null)
                {
                    query += "&zoom=" + System.Uri.EscapeDataString(maps.Location.Zoom);
                }
            }
            if ((_signingSecret != null))
            {
                query += "&signature=" + _signingSecret;
            }
            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessRequest(query);
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

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="query">The query to apply.</param>
        /// <returns>The returned byte array.</returns>
        private byte[] ProcessRequest(string query)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "?" + query.TrimStart(new char[] { '&' }));
            return Nequeo.Net.HttpDataClient.Request(constructedServiceUri);
        }
    }
}
