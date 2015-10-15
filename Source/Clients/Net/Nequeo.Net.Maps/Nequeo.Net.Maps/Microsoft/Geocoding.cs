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
    /// Geocoding model.
    /// </summary>
    public class Geocoding
    {
        /// <summary>
        /// Gets or sets the authentication result code.
        /// </summary>
        public string AuthenticationResultCode { get; set; }

        /// <summary>
        /// Gets or sets the brand logo uri.
        /// </summary>
        public string BrandLogoUri { get; set; }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Gets or sets the array of resource sets.
        /// </summary>
        public ResourceSet[] ResourceSets { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the trace ID.
        /// </summary>
        public string TraceId { get; set; }
    }

    /// <summary>
    /// Resource set model.
    /// </summary>
    public class ResourceSet
    {
        /// <summary>
        /// Gets or sets the estimated total.
        /// </summary>
        public int EstimatedTotal { get; set; }

        /// <summary>
        /// Gets or sets the resource array.
        /// </summary>
        public Resource[] Resources { get; set; }
    }

    /// <summary>
    /// Resource model.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string __type { get; set; }

        /// <summary>
        /// Gets or sets the b-box.
        /// </summary>
        public double[] Bbox { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets the confidence.
        /// </summary>
        public string Confidence { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the geocode points.
        /// </summary>
        public GeocodePoint[] GeocodePoints { get; set; }

        /// <summary>
        /// Gets or sets the match codes.
        /// </summary>
        public string[] MatchCodes { get; set; }
    }

    /// <summary>
    /// Point model.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public double[] Coordinates { get; set; }
    }

    /// <summary>
    /// Address model.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        public string AdminDistrict { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        public string AdminDistrict2 { get; set; }

        /// <summary>
        /// Gets or sets the country region.
        /// </summary>
        public string CountryRegion { get; set; }

        /// <summary>
        /// Gets or sets the formatted address.
        /// </summary>
        public string FormattedAddress { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the neighborhood.
        /// </summary>
        public string Neighborhood { get; set; }

        /// <summary>
        /// Gets or sets the landmark.
        /// </summary>
        public string Landmark { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; }
    }

    /// <summary>
    /// Geocode point model.
    /// </summary>
    public class GeocodePoint
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public double[] Coordinates { get; set; }

        /// <summary>
        /// Gets or sets the calculation method.
        /// </summary>
        public string CalculationMethod { get; set; }

        /// <summary>
        /// Gets or sets the usage types.
        /// </summary>
        public string[] UsageTypes { get; set; }
    }
}
