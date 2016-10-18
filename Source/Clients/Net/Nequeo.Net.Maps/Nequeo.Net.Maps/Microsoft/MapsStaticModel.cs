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
    /// Maps static.
    /// </summary>
    public class MapsStatic
    {
        /// <summary>
        /// Gets or sets CenterPoint Required. A point on the Earth where the map is centered. A Point value (latitude and longitude) pair (e.g. "40.714728,-73.998672") or a 
        /// string address (e.g. "city hall, new york, ny") identifying a unique location on the face of the earth.
        /// </summary>
        /// <remarks>
        /// Point
        ///
        /// A point on the Earth specified by a latitude and longitude.
        ///
        /// The coordinates are double values that are separated by commas and are specified in the following order.
        /// Latitude, Longitude
        /// Use the following ranges of values: 
        /// 
        /// Latitude (degrees): [-90, +90]
        /// Longitude(degrees): [-180,+180]
        ///
        /// Example: 47.610679194331169,-122.10788659751415
        ///
        /// -----------
        /// BoundingBox
        ///
        /// A rectangular area on the Earth.
        ///
        /// A bounding box is defined by two latitudes and two longitudes that represent the four sides of a rectangular 
        /// area on the Earth.Use the following syntax to specify a bounding box.
        ///
        /// South Latitude, West Longitude, North Latitude, East Longitude
        ///
        /// Example: 45.219,-122.325,47.610,-122.107
        ///
        /// -------
        /// Address
        ///
        /// Details about a point on the Earth that has additional location information.
        ///
        /// An address can contain the following fields: address line, locality, neighborhood, admin district, admin district 2, 
        /// formatted address, postal code and country or region.For descriptions see the Address Fields section below.
        ///
        /// -----------
        /// addressLine
        ///
        /// The official street line of an address relative to the area, as specified by the Locality, or PostalCode, 
        /// properties.Typical use of this element would be to provide a street address or any official address.
        ///
        /// Example: 1 Microsoft Way
        ///
        /// locality
        ///
        /// A string specifying the populated place for the address. This typically refers to a city, but 
        /// may refer to a suburb or a neighborhood in certain countries.
        ///
        /// Example: Seattle
        ///
        /// neighborhood
        ///
        /// A string specifying the neighborhood for an address. 
        /// You must specify includeNeighborhood = 1 in your request to return the neighborhood.
        ///
        /// Example: Ballard
        ///
        /// adminDistrict
        ///
        /// A string specifying the subdivision name in the country or region for an address. This element is 
        /// typically treated as the first order administrative subdivision, but in some cases it is the second, 
        /// third, or fourth order subdivision in a country, dependency, or region. 
        ///
        /// Example: WA
        ///
        /// adminDistrict2
        ///
        /// A string specifying the subdivision name in the country or region for an address. This element is 
        /// used when there is another level of subdivision information for a location, such as the county.
        ///
        /// Example: King
        ///
        /// formattedAddress
        ///
        /// A string specifying the complete address.This address may not include the country or region.
        /// 
        /// Example: 1 Microsoft Way, Redmond, WA 98052-8300
        ///
        /// postalCode
        ///
        /// A string specifying the post code, postal code, or ZIP Code of an address.
        ///
        /// Example: 98178
        ///
        /// countryRegion
        ///
        /// A string specifying the country or region name of an address.
        ///
        /// Example: United States
        ///
        /// countryRegionIso2
        ///
        /// A string specifying the two-letter ISO country code.
        /// You must specify include = ciso2 in your request to return this ISO country code.
        ///
        /// Example: US
        ///
        /// landmark
        ///
        /// A string specifying the name of the landmark when there is a landmark associated with an address.
        ///
        /// Example: Eiffel Tower
        /// </remarks>
        public string CenterPoint { get; set; }

        /// <summary>
        /// Gets or sets DeclutterPins Optional. Specifies whether to change the display of overlapping pushpins so that they display separately on a map.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	1: Declutter pusphpin icons.
        ///	0 [default]: Do not declutter pushpin icons.
        /// </remarks>
        public string DeclutterPins { get; set; }

        /// <summary>
        /// Gets or sets Format Optional. The image format to use for the static map.
        /// </summary>
        /// <remarks>
        /// One of the following image format values:
        /// 
        ///	gif: Use GIF image format.
        ///	jpeg: Use JPEG image format. JPEG format is the default for Road, Aerial and AerialWithLabels imagery.
        ///	png: Use PNG image format. PNG is the default format for CollinsBart and OrdnanceSurvey imagery.
        /// </remarks>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets ImagerySet Required. The type of imagery.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	Aerial – Aerial imagery.
        ///	AerialWithLabels – Aerial imagery with a road overlay.
        ///	Road – Roads without additional imagery.
        ///	OrdnanceSurvey - Ordnance Survey imagery.This imagery is visible only for the London area.
        ///	CollinsBart – Collins Bart imagery.This imagery is visible only for the London area.
        /// </remarks>
        public string ImagerySet { get; set; }

        /// <summary>
        /// Gets or sets MapArea Required when a center point or set of route points are not specified. The geographic area to display on the map.
        /// A rectangular area specified as a bounding box. (latitude and longitude) pair (e.g. 45.219, -122.325, 47.610, -122.107).
        /// </summary>
        /// <remarks>
        /// Point
        ///
        /// A point on the Earth specified by a latitude and longitude.
        ///
        /// The coordinates are double values that are separated by commas and are specified in the following order.
        /// Latitude, Longitude
        /// Use the following ranges of values: 
        /// 
        /// Latitude (degrees): [-90, +90]
        /// Longitude(degrees): [-180,+180]
        ///
        /// Example: 47.610679194331169,-122.10788659751415
        ///
        /// -----------
        /// BoundingBox
        ///
        /// A rectangular area on the Earth.
        ///
        /// A bounding box is defined by two latitudes and two longitudes that represent the four sides of a rectangular 
        /// area on the Earth.Use the following syntax to specify a bounding box.
        ///
        /// South Latitude, West Longitude, North Latitude, East Longitude
        ///
        /// Example: 45.219,-122.325,47.610,-122.107
        ///
        /// -------
        /// Address
        ///
        /// Details about a point on the Earth that has additional location information.
        ///
        /// An address can contain the following fields: address line, locality, neighborhood, admin district, admin district 2, 
        /// formatted address, postal code and country or region.For descriptions see the Address Fields section below.
        ///
        /// -----------
        /// addressLine
        ///
        /// The official street line of an address relative to the area, as specified by the Locality, or PostalCode, 
        /// properties.Typical use of this element would be to provide a street address or any official address.
        ///
        /// Example: 1 Microsoft Way
        ///
        /// locality
        ///
        /// A string specifying the populated place for the address. This typically refers to a city, but 
        /// may refer to a suburb or a neighborhood in certain countries.
        ///
        /// Example: Seattle
        ///
        /// neighborhood
        ///
        /// A string specifying the neighborhood for an address. 
        /// You must specify includeNeighborhood = 1 in your request to return the neighborhood.
        ///
        /// Example: Ballard
        ///
        /// adminDistrict
        ///
        /// A string specifying the subdivision name in the country or region for an address. This element is 
        /// typically treated as the first order administrative subdivision, but in some cases it is the second, 
        /// third, or fourth order subdivision in a country, dependency, or region. 
        ///
        /// Example: WA
        ///
        /// adminDistrict2
        ///
        /// A string specifying the subdivision name in the country or region for an address. This element is 
        /// used when there is another level of subdivision information for a location, such as the county.
        ///
        /// Example: King
        ///
        /// formattedAddress
        ///
        /// A string specifying the complete address.This address may not include the country or region.
        /// 
        /// Example: 1 Microsoft Way, Redmond, WA 98052-8300
        ///
        /// postalCode
        ///
        /// A string specifying the post code, postal code, or ZIP Code of an address.
        ///
        /// Example: 98178
        ///
        /// countryRegion
        ///
        /// A string specifying the country or region name of an address.
        ///
        /// Example: United States
        ///
        /// countryRegionIso2
        ///
        /// A string specifying the two-letter ISO country code.
        /// You must specify include = ciso2 in your request to return this ISO country code.
        ///
        /// Example: US
        ///
        /// landmark
        ///
        /// A string specifying the name of the landmark when there is a landmark associated with an address.
        ///
        /// Example: Eiffel Tower
        /// </remarks>
        public string MapArea { get; set; }

        /// <summary>
        /// Gets or sets MapLayer Optional. A display layer that renders on top of the imagery set.
        /// </summary>
        /// <remarks>
        /// The only value for this parameter is TrafficFlow.
        /// </remarks>
        public string MapLayer { get; set; }

        /// <summary>
        /// Gets or sets MapSize Optional. The width and height in pixels of the static map output (e.g. 100,600).
        /// </summary>
        /// <remarks>
        /// A string that contains a width and a height separated by a comma. The width must be between 80 and 900 pixels 
        /// and the height must be between 80 and 834 pixels. The default map size for static maps is 350 pixels by 350 pixels. 
        /// </remarks>
        public string MapSize { get; set; }

        /// <summary>
        /// Gets or sets PushPin Optional. One or more pushpin locations to display on the map.
        /// </summary>
        /// <remarks>
        /// A series of values that include a Point value (latitude and longitude) with options to add a label 
        /// of up to three (3) characters and to specify an icon style. For more information about specifying pushpins, 
        /// see Pushpin Syntax and Icon Styles. You can specify up to 18 pushpins within a URL and 100 if you use the 
        /// HTTP POST method and specify the pushpins in the body of the request. See the Examples section for examples.
        ///
        /// Example: 47.610,-122.107;5;P10
        /// </remarks>
        public PushPin[] PushPins { get; set; }

        /// <summary>
        /// Push pin labels.
        /// </summary>
        public class PushPin
        {
            /// <summary>
            /// Gets or sets Location Required. A Point value (latitude and longitude) pair (e.g. "40.714728,-73.998672")
            /// </summary>
            public string Location { get; set; }

            /// <summary>
            /// Gets or sets Location Optional. A number from 0 to 112. see https://msdn.microsoft.com/en-us/library/ff701719.aspx
            /// </summary>
            public string IconStyle { get; set; }

            /// <summary>
            /// Gets or sets Label Optional. A label to place on the push pin. A label can have up to three (3) characters.
            /// </summary>
            public string Label { get; set; }
        }

        /// <summary>
        /// Gets or sets MapMetadata Optional. Specifies whether to return metadata for the static map instead of the image. The static map 
        /// metadata includes the size of the static map and the placement and size of the pushpins on the static map.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	1: Return metadata for the specific image.An image is not returned.
        ///	0: Do not return metadata. [default]
        ///
        /// When you request metadata, the response returns metadata for the map instead of the 
        /// map image.For more information about the static map metadata, see https://msdn.microsoft.com/en-us/library/hh667439.aspx
        /// </remarks>
        public string MapMetadata { get; set; }

        /// <summary>
        /// Gets or sets Query Required. A query string that is used to determine the map location to display.
        /// </summary>
        /// <remarks>
        /// A string that contains query terms for the location of the static map.
        ///
        /// Example: Seattle Center
        /// </remarks>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets ZoomLevel Required. The level of zoom to display. An integer between 0 and 21.
        /// </summary>
        public string ZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets HighlightEntity Optional. Highlights a polygon for an entity. 1 = Highlight polygon is on.
        /// </summary>
        public string HighlightEntity { get; set; }

        /// <summary>
        /// Gets or sets EntityType Optional. Indicates the type of entity that should be highlighted. 
        /// The entity of this type that contains the <see cref="MapsStatic.CenterPoint"/> will be highlighted.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        public MapsStaticRoute Route { get; set; }
    }

    /// <summary>
    /// Maps static route.
    /// </summary>
    public class MapsStaticRoute
    {
        /// <summary>
        /// Gets or sets Avoid Optional. Specifies the road types to minimize or avoid when the route is created for the driving travel mode.
        /// </summary>
        /// <remarks>
        /// Examples: 
        ///
        /// highways
        /// highways, tolls
        ///
        /// A comma-separated list of values that limit the use of highways and toll roads in the route. 
        /// In the definitions below, “highway” also refers to a “limited-access highway”. 
        ///
        /// If no values are specified, highways and tolls are allowed in the route.
        /// 
        ///	highways: Avoids the use of highways in the route. 
        ///	tolls: Avoids the use of toll roads in the route.
        ///	minimizeHighways: Minimizes (tries to avoid) the use of highways in the route.
        ///	minimizeTolls: Minimizes (tries to avoid) the use of toll roads in the route.
        /// </remarks>
        public string Avoid { get; set; }

        /// <summary>
        /// Gets or sets DistanceBeforeFirstTurn Optional. Specifies the distance before the first turn is allowed in the route. 
        /// This option only applies to the driving travel mode.
        /// </summary>
        public string DistanceBeforeFirstTurn { get; set; }

        /// <summary>
        /// Gets or sets DateTime Required when the travel mode is Transit. The timeType parameter identifies 
        /// the desired transit time, such as arrival time or departure time. The transit time type is specified 
        /// by the timeType parameter.
        /// </summary>
        /// <remarks>
        /// Examples:
        /// 
        ///	03/01/2011 05:42:00 
        ///	05:42:00 [assumes the current day]
        ///	03/01/2011 [assumes the current time]
        /// </remarks>
        public string DateTime { get; set; }

        /// <summary>
        /// Gets or sets MaxSolutions Optional. Specifies the maximum number of transit routes to return.
        /// </summary>
        /// <remarks>
        /// A string that contains an integer value. The default value is 1.
        ///
        /// Example: 3
        ///
        /// This parameter is only supported for the Transit travel mode.
        /// </remarks>
        public string MaxSolutions { get; set; }

        /// <summary>
        /// Gets or sets Optimize Optional. Specifies what parameters to use to optimize the route on the map.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	distance: The route is calculated to minimize the distance.Traffic information is not used.
        ///	time[default]: The route is calculated to minimize the time.Traffic information is not used.
        ///	timeWithTraffic: The route is calculated to minimize the time and uses current traffic information.
        /// </remarks>
        public string Optimize { get; set; }

        /// <summary>
        /// Gets or sets TimeType Required when the travel mode is Transit. Specifies how to interpret the 
        /// date and transit time value that is specified by the dateTime parameter.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	Arrival: The dateTime parameter contains the desired arrival time for a transit request.
        ///	Departure: The dateTime parameter contains the desired departure time for a transit request.
        ///	LastAvailable: The dateTime parameter contains the latest departure time available for a transit request.
        /// </remarks>
        public string TimeType { get; set; }

        /// <summary>
        /// Gets or sets TravelMode Optional. The mode of travel for the route.
        /// </summary>
        /// <remarks>
        /// One of the following values:
        /// 
        ///	Driving [default]
        ///	Walking
        ///	Transit
        /// </remarks>
        public string TravelMode { get; set; }

        /// <summary>
        /// Gets or sets WayPoint Required. Specifies two or more locations that define the route and that are in sequential order.
        /// A waypoint location can be specified as a point, a landmark, or an address. You can optionally specify an icon style 
        /// and add a label of up to three (3) characters for each waypoint. For a list of icon styles, see Pushpin Syntax and 
        /// Icon Styles. For more information about Point values, see Location and Area Types. 
        /// </summary>
        public WayPoint[] WayPoints { get; set; }

        /// <summary>
        /// Push pin labels.
        /// </summary>
        public class WayPoint
        {
            /// <summary>
            /// Gets or sets Location Required. A Point value (latitude and longitude) pair (e.g. "40.714728,-73.998672")
            /// </summary>
            /// <remarks>
            /// Point
            ///
            /// A point on the Earth specified by a latitude and longitude.
            ///
            /// The coordinates are double values that are separated by commas and are specified in the following order.
            /// Latitude, Longitude
            /// Use the following ranges of values: 
            /// 
            /// Latitude (degrees): [-90, +90]
            /// Longitude(degrees): [-180,+180]
            ///
            /// Example: 47.610679194331169,-122.10788659751415
            ///
            /// -----------
            /// BoundingBox
            ///
            /// A rectangular area on the Earth.
            ///
            /// A bounding box is defined by two latitudes and two longitudes that represent the four sides of a rectangular 
            /// area on the Earth.Use the following syntax to specify a bounding box.
            ///
            /// South Latitude, West Longitude, North Latitude, East Longitude
            ///
            /// Example: 45.219,-122.325,47.610,-122.107
            ///
            /// -------
            /// Address
            ///
            /// Details about a point on the Earth that has additional location information.
            ///
            /// An address can contain the following fields: address line, locality, neighborhood, admin district, admin district 2, 
            /// formatted address, postal code and country or region.For descriptions see the Address Fields section below.
            ///
            /// -----------
            /// addressLine
            ///
            /// The official street line of an address relative to the area, as specified by the Locality, or PostalCode, 
            /// properties.Typical use of this element would be to provide a street address or any official address.
            ///
            /// Example: 1 Microsoft Way
            ///
            /// locality
            ///
            /// A string specifying the populated place for the address. This typically refers to a city, but 
            /// may refer to a suburb or a neighborhood in certain countries.
            ///
            /// Example: Seattle
            ///
            /// neighborhood
            ///
            /// A string specifying the neighborhood for an address. 
            /// You must specify includeNeighborhood = 1 in your request to return the neighborhood.
            ///
            /// Example: Ballard
            ///
            /// adminDistrict
            ///
            /// A string specifying the subdivision name in the country or region for an address. This element is 
            /// typically treated as the first order administrative subdivision, but in some cases it is the second, 
            /// third, or fourth order subdivision in a country, dependency, or region. 
            ///
            /// Example: WA
            ///
            /// adminDistrict2
            ///
            /// A string specifying the subdivision name in the country or region for an address. This element is 
            /// used when there is another level of subdivision information for a location, such as the county.
            ///
            /// Example: King
            ///
            /// formattedAddress
            ///
            /// A string specifying the complete address.This address may not include the country or region.
            /// 
            /// Example: 1 Microsoft Way, Redmond, WA 98052-8300
            ///
            /// postalCode
            ///
            /// A string specifying the post code, postal code, or ZIP Code of an address.
            ///
            /// Example: 98178
            ///
            /// countryRegion
            ///
            /// A string specifying the country or region name of an address.
            ///
            /// Example: United States
            ///
            /// countryRegionIso2
            ///
            /// A string specifying the two-letter ISO country code.
            /// You must specify include = ciso2 in your request to return this ISO country code.
            ///
            /// Example: US
            ///
            /// landmark
            ///
            /// A string specifying the name of the landmark when there is a landmark associated with an address.
            ///
            /// Example: Eiffel Tower
            /// </remarks>
            public string Location { get; set; }

            /// <summary>
            /// Gets or sets Location Optional. A number from 0 to 112. see https://msdn.microsoft.com/en-us/library/ff701719.aspx
            /// </summary>
            public string IconStyle { get; set; }

            /// <summary>
            /// Gets or sets Label Optional. A label to place on the push pin. A label can have up to three (3) characters.
            /// </summary>
            public string Label { get; set; }
        }
    }
}
