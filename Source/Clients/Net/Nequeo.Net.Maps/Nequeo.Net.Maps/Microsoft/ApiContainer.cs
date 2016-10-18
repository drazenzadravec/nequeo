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

namespace Nequeo.Net.Maps.Microsoft
{
    /// <summary>
    /// Microsoft api map container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Microsoft api map container.
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
        /// Get the geocoding information for the address.
        /// </summary>
        /// <param name="address">The address to get geocoding for.</param>
        /// <returns>The geocoding address.</returns>
        public Geocoding GetGeocode(string address)
        {
            string query = "";
            query += "&q=" + System.Uri.EscapeDataString(address);

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

            if (maps.DeclutterPins != null)
            {
                query += "&declutter=" + System.Uri.EscapeDataString(maps.DeclutterPins);
            }
            if (maps.Format != null)
            {
                query += "&format=" + System.Uri.EscapeDataString(maps.Format);
            }
            if (maps.MapArea != null)
            {
                query += "&mapArea=" + System.Uri.EscapeDataString(maps.MapArea);
            }
            if (maps.MapLayer != null)
            {
                query += "&mapLayer=" + System.Uri.EscapeDataString(maps.MapLayer);
            }
            if (maps.MapSize != null)
            {
                query += "&mapSize=" + System.Uri.EscapeDataString(maps.MapSize);
            }
            if (maps.PushPins != null)
            {
                foreach (MapsStatic.PushPin pushpin in maps.PushPins)
                {
                    string pushpinQuery = "";

                    if (pushpin.Location != null)
                    {
                        pushpinQuery += pushpin.Location + ";";
                    }
                    if (pushpin.IconStyle != null || pushpin.Label != null)
                    {
                        if (pushpin.IconStyle != null)
                        {
                            pushpinQuery += pushpin.IconStyle + ";";
                        }
                        else
                        {
                            pushpinQuery += ";";
                        }
                        if (pushpin.Label != null)
                        {
                            pushpinQuery += pushpin.Label + ";";
                        }
                    }
                    if (!String.IsNullOrEmpty(pushpinQuery))
                    {
                        query += "&pushpin=" + System.Uri.EscapeDataString(pushpinQuery.TrimEnd(';'));
                    }
                }
            }
            if (maps.MapMetadata != null)
            {
                query += "&mmd=" + System.Uri.EscapeDataString(maps.MapMetadata);
            }
            if (maps.HighlightEntity != null)
            {
                query += "&he=" + System.Uri.EscapeDataString(maps.HighlightEntity);
            }
            if (maps.EntityType != null)
            {
                query += "&entityType=" + System.Uri.EscapeDataString(maps.EntityType);
            }
            if (maps.Route != null)
            {
                if (maps.Route.Avoid != null)
                {
                    query += "&avoid=" + System.Uri.EscapeDataString(maps.Route.Avoid);
                }
                if (maps.Route.DistanceBeforeFirstTurn != null)
                {
                    query += "&dbft=" + System.Uri.EscapeDataString(maps.Route.DistanceBeforeFirstTurn);
                }
                if (maps.Route.DateTime != null)
                {
                    query += "&dateTime=" + System.Uri.EscapeDataString(maps.Route.DateTime);
                }
                if (maps.Route.MaxSolutions != null)
                {
                    query += "&maxSolns=" + System.Uri.EscapeDataString(maps.Route.MaxSolutions);
                }
                if (maps.Route.Optimize != null)
                {
                    query += "&optimize=" + System.Uri.EscapeDataString(maps.Route.Optimize);
                }
                if (maps.Route.TimeType != null)
                {
                    query += "&timeType=" + System.Uri.EscapeDataString(maps.Route.TimeType);
                }
                if (maps.Route.TravelMode != null)
                {
                    query += "&travelMode=" + System.Uri.EscapeDataString(maps.Route.TravelMode);
                }
                if (maps.Route.WayPoints != null)
                {
                    int wpIndex = 1;

                    foreach (MapsStaticRoute.WayPoint waypoint in maps.Route.WayPoints)
                    {
                        string waypointQuery = "";

                        if (waypoint.Location != null)
                        {
                            waypointQuery += waypoint.Location + ";";
                        }
                        if (waypoint.IconStyle != null || waypoint.Label != null)
                        {
                            if (waypoint.IconStyle != null)
                            {
                                waypointQuery += waypoint.IconStyle + ";";
                            }
                            else
                            {
                                waypointQuery += ";";
                            }
                            if (waypoint.Label != null)
                            {
                                waypointQuery += waypoint.Label + ";";
                            }
                        }
                        if (!String.IsNullOrEmpty(waypointQuery))
                        {
                            query += "&wp." + wpIndex.ToString() + "=" + System.Uri.EscapeDataString(waypointQuery.TrimEnd(';'));
                            wpIndex++;
                        }
                    }
                }
            }
            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessRequest(query, 
                (maps.ImagerySet != null ? System.Uri.EscapeDataString(maps.ImagerySet) : null),
                (maps.Query != null ? System.Uri.EscapeDataString(maps.Query) : null),
                (maps.CenterPoint != null ? System.Uri.EscapeDataString(maps.CenterPoint) : null),
                (maps.ZoomLevel != null ? System.Uri.EscapeDataString(maps.ZoomLevel) : null));
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
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "?" + query.TrimStart(new char[] { '&' }) + "&o=" + serviceName);
            return Nequeo.Net.HttpJsonClient.Request<T>(constructedServiceUri);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="query">The query to apply.</param>
        /// <param name="serviceName">The service name to call.</param>
        /// <returns>The returned type.</returns>
        private T ProcessXmlRequest<T>(string query, string serviceName = "xml")
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "?" + query.TrimStart(new char[] { '&' }) +"&o=" + serviceName);
            return Nequeo.Net.HttpXmlClient.Request<T>(constructedServiceUri);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="query">The query to apply.</param>
        /// <param name="imagerySet">The imagery set to display.</param>
        /// <param name="querySet">The query set to display.</param>
        /// <param name="centerPoint">The center point display.</param>
        /// <param name="zoomLevel">The zoom level display.</param>
        /// <returns>The returned byte array.</returns>
        private byte[] ProcessRequest(string query, string imagerySet = "imagerySet", string querySet = "querySet", string centerPoint = "centerPoint", string zoomLevel = "1")
        {
            if (!String.IsNullOrEmpty(querySet))
            {
                // Construct the URI.
                Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + imagerySet +
                    (String.IsNullOrEmpty(querySet) ? "/" : "/" + querySet) + "?" + query.TrimStart(new char[] { '&' }));
                return Nequeo.Net.HttpDataClient.Request(constructedServiceUri);
            }
            else
            {
                if (!String.IsNullOrEmpty(centerPoint) && !String.IsNullOrEmpty(zoomLevel))
                {
                    // Construct the URI.
                    Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + imagerySet +
                        (String.IsNullOrEmpty(centerPoint) ? "/" : "/" + centerPoint) +
                        (String.IsNullOrEmpty(zoomLevel) ? "/" : "/" + zoomLevel) +
                        "?" + query.TrimStart(new char[] { '&' }));
                    return Nequeo.Net.HttpDataClient.Request(constructedServiceUri);
                }
                else if(!String.IsNullOrEmpty(centerPoint))
                {
                    // Construct the URI.
                    Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + imagerySet +
                        (String.IsNullOrEmpty(centerPoint) ? "/" : "/" + centerPoint) + "?" + query.TrimStart(new char[] { '&' }));
                    return Nequeo.Net.HttpDataClient.Request(constructedServiceUri);
                }
                else
                {
                    // Construct the URI.
                    Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + imagerySet +
                        "?" + query.TrimStart(new char[] { '&' }));
                    return Nequeo.Net.HttpDataClient.Request(constructedServiceUri);
                }
            }
        }
    }
}
