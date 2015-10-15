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
    /// Geocoding model.
    /// </summary>
    public class Geocoding
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the results array.
        /// </summary>
        public GeocodingResult[] Results { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Error_Message { get; set; }
    }

    /// <summary>
    /// Geocoding result model.
    /// </summary>
    public class GeocodingResult
    {
        /// <summary>
        /// Gets or sets the address component array.
        /// </summary>
        public AddressComponent[] Address_Components { get; set; }

        /// <summary>
        /// Gets or sets the formatted address.
        /// </summary>
        public string Formatted_Address { get; set; }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets the place ID.
        /// </summary>
        public string Place_Id { get; set; }

        /// <summary>
        /// Gets or sets the address type array.
        /// </summary>
        public string[] Types { get; set; }
    }

    /// <summary>
    /// Address component.
    /// </summary>
    public class AddressComponent
    {
        /// <summary>
        /// Gets or sets the long name.
        /// </summary>
        public string Long_Name { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string Short_Name { get; set; }

        /// <summary>
        /// Gets or sets the address type array.
        /// </summary>
        public string[] Types { get; set; }
    }

    /// <summary>
    /// Geometry model.
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the location type.
        /// </summary>
        public string Location_Type { get; set; }

        /// <summary>
        /// Gets or sets the view port.
        /// </summary>
        public Viewport Viewport { get; set; }
    }

    /// <summary>
    /// Location model.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public string Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public string Lng { get; set; }
    }

    /// <summary>
    /// Viewport model.
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// Gets or sets the morth east location.
        /// </summary>
        public Location NorthEast { get; set; }

        /// <summary>
        /// Gets or sets the south west location.
        /// </summary>
        public Location SouthWest { get; set; }
    }
}
