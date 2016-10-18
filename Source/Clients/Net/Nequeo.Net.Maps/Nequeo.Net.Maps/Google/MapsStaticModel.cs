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
    /// Maps static.
    /// </summary>
    public class MapsStatic
    {
        /// <summary>
        /// Gets or sets size (required) defines the rectangular dimensions of the map image. This parameter takes a string of the form 
        /// {horizontal_value}x{vertical_value}. For example, 500x400 defines a map 500 pixels wide by 400 pixels high. Maps smaller 
        /// than 180 pixels in width will display a reduced-size Google logo. This parameter is affected by the scale parameter, 
        /// described below; the final output size is the product of the size and scale values.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets scale (optional) affects the number of pixels that are returned. scale=2 returns twice as many pixels as scale=1 
        /// while retaining the same coverage area and level of detail (i.e. the contents of the map don't change). This is useful when 
        /// developing for high-resolution displays, or when generating a map for printing. The default value is 1. Accepted values are 2 
        /// and 4 (4 is only available to Google Maps API for Work customers.).
        /// </summary>
        public string Scale { get; set; }

        /// <summary>
        /// Gets or sets format (optional) defines the format of the resulting image. By default, the Google Static Maps API creates PNG images. 
        /// There are several possible formats including GIF, JPEG and PNG types. Which format you use depends on how you intend to present the 
        /// image. JPEG typically provides greater compression, while GIF and PNG provide greater detail. 
        /// </summary>
        /// <remarks>
        /// Images may be returned in several common web graphics formats: GIF, JPEG and PNG. The format parameter takes one of the following values:
        /// 
        /// png8 or png(default) specifies the 8-bit PNG format.
        /// png32 specifies the 32-bit PNG format.
        /// gif specifies the GIF format.
        /// jpg specifies the JPEG compression format.
        /// jpg-baseline specifies a non-progressive JPEG compression format.
        ///
        /// jpg and jpg-baseline typically provide the smallest image size, though they do so through "lossy" compression which may degrade the image. 
        /// gif, png8 and png32 provide lossless compression.
        ///
        /// Most JPEG images are progressive, meaning that they load a coarser image earlier and refine the image resolution as more data arrives. 
        /// This allows images to be loaded quickly in webpages and is the most widespread use of JPEG currently.However, some uses of JPEG (especially printing) 
        /// require non-progressive(baseline) images.In such cases, you may want to use the jpg-baseline format, which is non-progressive.
        /// </remarks>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets maptype (optional) defines the type of map to construct. There are several possible maptype values, including roadmap, satellite, 
        /// hybrid, and terrain.
        /// </summary>
        /// <remarks>
        /// The Google Static Maps API creates maps in several formats, listed below:
        /// 
        /// roadmap (default) specifies a standard roadmap image, as is normally shown on the Google Maps website.If no maptype value is specified, the Google Static Maps API serves roadmap tiles by default.
        /// satellite specifies a satellite image.
        /// terrain specifies a physical relief map image, showing terrain and vegetation.
        /// hybrid specifies a hybrid of the satellite and roadmap image, showing a transparent layer of major streets and place names on the satellite image.
        /// </remarks>
        public string MapType { get; set; }

        /// <summary>
        /// Gets or sets language (optional) defines the language to use for display of labels on map tiles. Note that this parameter is only supported for 
        /// some country tiles; if the specific language requested is not supported for the tile set, then the default language for that tileset will be used.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets region (optional) defines the appropriate borders to display, based on geo-political sensitivities. 
        /// Accepts a region code specified as a two-character ccTLD ('top-level domain') value.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the location (required if markers <seealso cref="MapsStaticFeature"/> not present).
        /// </summary>
        public MapsStaticLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the feature.
        /// </summary>
        public MapsStaticFeature Feature { get; set; }
    }

    /// <summary>
    /// Maps static locations.
    /// </summary>
    public class MapsStaticLocation
    {
        /// <summary>
        /// Gets or sets center (required if markers <seealso cref="MapsStaticFeature"/> not present) defines the center of the map, equidistant from all edges of the map. 
        /// This parameter takes a location as either a comma-separated {latitude,longitude} pair (e.g. "40.714728,-73.998672") or a 
        /// string address (e.g. "city hall, new york, ny") identifying a unique location on the face of the earth.
        /// </summary>
        public string Center { get; set; }

        /// <summary>
        /// Gets or sets zoom (required if markers <seealso cref="MapsStaticFeature"/> not present) defines the zoom level of the map, which determines the magnification 
        /// level of the map. This parameter takes a numerical value corresponding to the zoom level of the region desired.
        /// </summary>
        public string Zoom { get; set; }
    }

    /// <summary>
    /// Maps static feature.
    /// </summary>
    public class MapsStaticFeature
    {
        /// <summary>
        /// Gets or sets markers (optional) define one or more markers to attach to the image at specified locations. This parameter takes a 
        /// single marker definition with parameters separated by the pipe character (|). Multiple markers may be placed within the same 
        /// markers parameter as long as they exhibit the same style; you may add additional markers of differing styles by adding additional 
        /// markers parameters. Note that if you supply markers for a map, you do not need to specify the (normally required) center and zoom 
        /// parameters.
        /// </summary>
        /// <remarks>
        /// The markers parameter defines a set of one or more markers at a set of locations. Each marker defined within a single markers 
        /// declaration must exhibit the same visual style; if you wish to display markers with different styles, you will need to supply 
        /// multiple markers parameters with separate style information.
        ///
        /// The markers parameter takes set of value assignments(marker descriptors) of the following format:
        ///
        /// markers=markerStyles|markerLocation1| markerLocation2|... etc.
        ///
        /// The set of markerStyles is declared at the beginning of the markers declaration and consists of zero or more style descriptors 
        /// separated by the pipe character(|), followed by a set of one or more locations also separated by the pipe character(|).
        ///
        /// Because both style information and location information is delimited via the pipe character, style information must appear first 
        /// in any marker descriptor.Once the Google Static Maps API server encounters a location in the marker descriptor, all other marker 
        /// parameters are assumed to be locations as well.
        /// 
        /// -------------
        /// Marker Styles
        ///
        /// The set of marker style descriptors is a series of value assignments separated by the pipe(|) character.This style descriptor 
        /// defines the visual attributes to use when displaying the markers within this marker descriptor.These style descriptors contain 
        /// the following key/value assignments:
        /// 
        /// size: (optional) specifies the size of marker from the set { tiny, mid, small }. If no size parameter is set, the marker will appear in its default (normal) size.
        /// color: (optional) specifies a 24-bit color(example: color= 0xFFFFCC) or a predefined color from the set {black, brown, green, purple, yellow, blue, gray, orange, red, white}.
        ///
        /// Note that transparencies(specified using 32-bit hex color values) are not supported in markers, though they are supported for paths.
        ///
        /// label: (optional) specifies a single uppercase alphanumeric character from the set {A-Z, 0-9}. (The requirement for uppercase characters is new to this version of the API.) Note that default and mid sized markers are the only markers capable of displaying an alphanumeric-character parameter. tiny and small markers are not capable of displaying an alphanumeric-character.
        ///
        /// Note:  instead of using these markers, you may wish to use your own custom icon. (For more information, see Custom Icons below.)
        ///
        /// ----------------
        /// Marker Locations
        ///
        /// Each marker descriptor must contain a set of one or more locations defining where to place the marker on the map.These locations may 
        /// be either specified as latitude/longitude values or as addresses.These locations are separated using the pipe character(|).
        ///
        /// The location parameters define the marker's location on the map. If the location is off the map, that marker will not appear in the 
        /// constructed image provided that center and zoom parameters are supplied. However, if these parameters are not supplied, the Google 
        /// Static Maps API server will automatically construct an image which contains the supplied markers.
        ///
        /// ------------
        /// Custom Icons
        ///
        /// Rather than use Google's marker icons, you are free to use your own custom icons instead. Custom icons are specified using the following 
        /// descriptors to the markers parameter: 
        /// 
        /// icon: specifies a URL to use as the marker's custom icon. Images may be in PNG, JPEG or GIF formats, though PNG is recommended.
        /// shadow: (default true) indicates that the Google Static Maps API service should construct an appropriate shadow for the image.This shadow is based on the image's visible region and its opacity/transparency.
        /// </remarks>
        public MapMarker[] Markers { get; set; }

        /// <summary>
        /// The map marker type.
        /// </summary>
        public class MapMarker
        {
            /// <summary>
            /// Gets or sets size (optional) specifies the size of marker from the set { tiny, mid, small }. 
            /// If no size parameter is set, the marker will appear in its default (normal) size.
            /// </summary>
            public string Size { get; set; }

            /// <summary>
            /// Gets or sets color (optional) specifies a 24-bit color(example: color= 0xFFFFCC) or a predefined color 
            /// from the set {black, brown, green, purple, yellow, blue, gray, orange, red, white}.
            /// </summary>
            public string Color { get; set; }

            /// <summary>
            /// Gets or sets label: (optional) specifies a single uppercase alphanumeric character from the set {A-Z, 0-9}. 
            /// (The requirement for uppercase characters is new to this version of the API.) Note that default and mid sized
            ///  markers are the only markers capable of displaying an alphanumeric-character parameter. tiny and small markers 
            /// are not capable of displaying an alphanumeric-character.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets In order to draw a path, the path parameter must also be passed two or more points. 
            /// The Google Static Maps API will then connect the path along those points, in the specified order. 
            /// Each pathPoint is denoted in the pathDescriptor separated by the | (pipe) character.
            /// (e.g. 40.737102,-73.990318 or 8th Avenue &amp; 34th St,New York,NY)
            /// </summary>
            public string[] Points { get; set; }
        }

        /// <summary>
        /// Gets or sets path (optional) defines a single path of two or more connected points to overlay on the image at specified locations. 
        /// This parameter takes a string of point definitions separated by the pipe character (|). You may supply additional paths by adding 
        /// additional path parameters. Note that if you supply a path for a map, you do not need to specify the (normally required) center 
        /// and zoom parameters.
        /// </summary>
        /// <remarks>
        /// The path parameter defines a set of one or more locations connected by a path to overlay on the map image. The path parameter 
        /// takes set of value assignments (path descriptors) of the following format:
        ///
        /// path=pathStyles|pathLocation1|pathLocation2|... etc.
        /// 
        /// Note that both path points are separated from each other using the pipe character(|). Because both style information and point 
        /// information is delimited via the pipe character, style information must appear first in any path descriptor.Once the Google Static 
        /// Maps API server encounters a location in the path descriptor, all other path parameters are assumed to be locations as well.
        ///
        /// -----------
        /// Path Styles
        ///
        /// The set of path style descriptors is a series of value assignments separated by the pipe(|) character.This style descriptor defines 
        /// the visual attributes to use when displaying the path.These style descriptors contain the following key/value assignments:
        /// 
        /// weight: (optional) specifies the thickness of the path in pixels.If no weight parameter is set, the path will appear in its default thickness(5 pixels).
        /// color: (optional) specifies a color either as a 24-bit(example: color= 0xFFFFCC) or 32-bit hexadecimal value(example: color= 0xFFFFCCFF), or from the set { black, brown, green, purple, yellow, blue, gray, orange, red, white }.
        ///         When a 32-bit hex value is specified, the last two characters specify the 8-bit alpha transparency value.This value varies between 00 
        ///         (completely transparent) and FF(completely opaque). Note that transparencies are supported in paths, though they are not supported for markers.
        /// fillcolor: (optional) indicates both that the path marks off a polygonal area and specifies the fill color to use as an overlay within that area.The set of locations following need not be a "closed" loop; the Google Static Maps API server will automatically join the first and last points.Note, however, that any stroke on the exterior of the filled area will not be closed unless you specifically provide the same beginning and end location.
        /// geodesic: (optional) indicates that the requested path should be interpreted as a geodesic line that follows the curvature of the Earth.When false, the path is rendered as a straight line in screen space. Defaults to false.
        ///
        /// Some example path definitions appear below:
        /// Thin blue line, 50% opacity: path=color:0x0000ff80|weight:1
        /// Solid red line: path= color:0xff0000ff|weight:5
        /// Solid thick white line: path=color:0xffffffff|weight:10
        ///
        /// These path styles are optional.If default attributes are desired, you may skip defining the path attributes; in that case, the path 
        /// descriptor's first "argument" will consist instead of the first declared point (location).
        ///
        /// -----------
        /// Path Points
        ///
        /// In order to draw a path, the path parameter must also be passed two or more points.The Google Static Maps API will then connect the 
        /// path along those points, in the specified order.Each pathPoint is denoted in the pathDescriptor separated by the | (pipe) character.
        ///
        /// -----------------
        /// Encoded Polylines
        ///
        /// Instead of a series of locations, you may instead declare a path as an encoded polyline by using the enc: prefix within the location 
        /// declaration of the path.Note that if you supply an encoded polyline path for a map, you do not need to specify the (normally required) 
        /// center and zoom parameters.
        ///
        /// As with standard paths, encoded polyline paths may also demarcate polygonal areas if a fillcolor argument is passed to the path parameter.
        /// </remarks>
        public MapPath Path { get; set; }

        /// <summary>
        /// The map path type.
        /// </summary>
        public class MapPath
        {
            /// <summary>
            /// Gets or sets weight (optional) specifies the thickness of the path in pixels. If no weight parameter 
            /// is set, the path will appear in its default thickness (5 pixels).
            /// </summary>
            public string Weight { get; set; }

            /// <summary>
            /// Gets or sets color (optional) specifies a color either as a 24-bit (example: color=0xFFFFCC) or 32-bit hexadecimal 
            /// value (example: color=0xFFFFCCFF), or from the set {black, brown, green, purple, yellow, blue, gray, orange, red, white}.
            /// </summary>
            public string Color { get; set; }

            /// <summary>
            /// Gets or sets fillcolor (optional) indicates both that the path marks off a polygonal area and specifies the 
            /// fill color to use as an overlay within that area. The set of locations following need not be a "closed" loop; 
            /// the Google Static Maps API server will automatically join the first and last points. Note, however, that any 
            /// stroke on the exterior of the filled area will not be closed unless you specifically provide the same beginning 
            /// and end location.
            /// </summary>
            public string FillColor { get; set; }

            /// <summary>
            /// Gets or sets geodesic (optional) indicates that the requested path should be interpreted as a geodesic 
            /// line that follows the curvature of the Earth. When false, the path is rendered as a straight line in 
            /// screen space. Defaults to false.
            /// </summary>
            public string Geodesic { get; set; }

            /// <summary>
            /// Gets or sets In order to draw a path, the path parameter must also be passed two or more points. 
            /// The Google Static Maps API will then connect the path along those points, in the specified order. 
            /// Each pathPoint is denoted in the pathDescriptor separated by the | (pipe) character.
            /// (e.g. 40.737102,-73.990318 or 8th Avenue &amp; 34th St,New York,NY)
            /// </summary>
            public string[] Points { get; set; }
        }

        /// <summary>
        /// Gets or sets visible (optional) specifies one or more locations that should remain visible on the map, though no markers or other 
        /// indicators will be displayed. Use this parameter to ensure that certain features or map locations are shown on the Google Static Maps API.
        /// </summary>
        /// <remarks>
        /// Specifying Locations
        /// 
        /// The Google Static Maps API must be able to precisely identify locations on the map, both to focus the map at the correct 
        /// location(using the center parameter) and/or to place any optional placemarks(using the markers parameter) at locations on the map.
        /// The Google Static Maps API uses numbers(latitude and longitude values) or strings(addresses) to specify these locations.These values 
        /// identify a geocoded location.
        ///
        /// Several parameters (such as the markers and path parameters) take multiple locations.In those cases, the locations are separated by the pipe(|) character.
        ///
        /// ---------
        /// Viewports
        ///
        /// Images may specify a viewport by specifying visible locations using the visible parameter.The visible parameter instructs the Google 
        /// Static Maps API service to construct a map such that the existing locations remain visible. (This parameter may be combined with 
        /// existing markers or paths to define a visible region as well.) Defining a viewport in this manner obviates the need to specify an 
        /// exact zoom level.
        /// </remarks>
        public string Visible { get; set; }

        /// <summary>
        /// Gets or sets style (optional) defines a custom style to alter the presentation of a specific feature (road, park, etc.) of the map. 
        /// This parameter takes feature and element arguments identifying the features to select and a set of style operations to apply to that 
        /// selection. You may supply multiple styles by adding additional style parameters. For more information, see Styled Maps below.
        /// </summary>
        /// <remarks>
        /// Styled maps allow you to customize the presentation of the standard Google map styles, changing the visual display of such elements as 
        /// roads, parks, and built-up areas to reflect a different style than that used in the default map type. These components are known as 
        /// features and a styled map allows you to select these features and apply visual styles to their display (including hiding them entirely). 
        /// With these changes, the map can be made to emphasize particular components or complement content within the surrounding page.
        ///
        /// A customized "styled" map consists of one or more specified styles, each indicated through a style parameter within the Google Static 
        /// Maps API request URL.Additional styles are specified by passing additional style parameters.A style consists of a selection(s) and a 
        /// set of rules to apply to that selection.The rules indicate what visual modification to make to the selection.
        ///
        /// Each style declaration consists of the following arguments, separated using a pipe("|") character within the style declaration:
        /// 
        /// feature: (optional) indicates what features to select for this style modification. (See Map Features below.) If no feature argument is passed, all features will be selected.
        /// element: (optional) indicates what sub-set of the selected features to select. (See Map Elements below.) If no element argument is passed, all elements of the given feature will be selected.
        ///
        /// Any following arguments indicate the rule(s)to apply to the above selection.All rules are applied in the order in which they appear within 
        /// the style declaration. (See Style Rules below.) Any number of rules may follow a feature selection, within the normal URL - length constraints 
        /// of the Google Static Maps API.
        /// 
        /// Note: the style declaration must specify the above arguments in the order stated.For example, a feature selection with two rules would appear as show below:
        ///
        /// style = feature:featureArgument | element:elementArgument | rule1:rule1Argument | rule2:rule2Argument
        ///
        /// ------------
        /// Map Features
        ///
        /// A map consists of a set of features, such as roads or parks. The feature types form a category tree, with feature:all as the root.Some common features are listed below:
        /// 
        /// feature:all (default) selects all features of the map.
        /// feature:road selects all roads on the map.
        /// feature:landscape selects all background landscapes on a map, which is often the area between roads, for example.In cities, landscape usually consists of built - up areas.
        ///
        /// The full list of features for selection within a map is documented in the  Google Maps JavaScript API reference.
        ///
        /// Some feature type categories contain sub - categories which are specified using a dotted notation(landscape.natural or road.local, for example).
        /// If the parent feature(road, for example) is specified, then styles applied to this selection will be applied to all roads, including sub-categories.
        ///
        /// --------------------
        /// Map Feature Elements
        ///
        /// Additionally, some features on a map typically consist of different elements. A road, for example, consists of not only the graphical line(geometry) 
        /// on the map, but the text denoting its name(labels) attached the map.Elements within features are selected by declaring an element argument.The 
        /// following element argument values are supported:
        /// 
        /// element:all (default) selects all elements of that feature.
        /// element:geometry selects only geometric elements of that feature.
        /// element:labels selects only textual labels associated with that feature.
        ///
        /// If no element argument is passed, styles will be applied to all elements of the feature regardless of element type.
        ///
        /// The following style declaration selects the labels for all local roads:
        ///
        /// style = feature:road.local | element:labels
        ///
        /// -----------
        /// Style Rules
        ///
        /// Style rules are formatting options which are applied to the features and elements specified within each style declaration.Each style declaration
        ///  must contain one or more operations separated using the pipe ("|") character.Each operation specifies its argument value using the colon (":") 
        /// character, and all operations are applied to the selection in the order in which they are specified.
        ///
        /// The following operation arguments, and the values that take, are currently supported:
        /// 
        /// hue: (an RGB hex string of format 0xRRGGBB) indicates the basic color to apply to the selection. (*See usage note below.)
        /// lightness: (a floating point value between - 100 and 100) indicates the percentage change in brightness of the element.Negative values increase darkness(where - 100 specifies black) while positive values increase brightness(where + 100 specifies white).
        /// saturation: (a floating point value between - 100 and 100) indicates the percentage change in intensity of the basic color to apply to the element.
        /// gamma: (a floating point value between 0.01 and 10.0, where 1.0 applies no correction) indicates the amount of gamma correction to apply to the element.Gammas modify the lightness of hues in a non-linear fashion, while unaffecting white or black values. Gammas are typically used to modify the contrast of multiple elements.For example, you could modify the gamma to increase or decrease the contrast between the edges and interiors of elements.Low gamma values(less than 1) increase contrast, while high values(greater than 1) decrease contrast.
        /// inverse_lightness: true simply inverts the existing lightness.
        /// visibility: (on, off, or simplified) indicates whether and how the element appears on the map. visibility: simplified indicates that the map should simplify the presentation of those elements as it sees fit. (A simplified road structure may show fewer roads, for example.)
        ///
        /// Style rules must be applied as separate, distinct operations, and are applied in the order they appear within the style declaration. Order is important, 
        /// as some operations are not commutative. Features and/ or elements that are modified through style operations(usually) already have existing styles; the 
        /// operations act on those existing styles, if present.
        ///
        /// Note that we use the  Hue, Saturation, Lightness(HSL) model to denote color within the styler operations. These operations to define color are 
        /// common within graphic design. Hue indicates the basic color, saturation indicates the intensity of that color, and lightness indicates the relative 
        /// amount of white or black in the constituent color.All three HSL values can be mapped to RGB values (and vice versa). Gamma correction acts to modify
        /// saturation over the color space, generally to increase or decrease contrast.Additionally, the HSL model defines color within a coordinate space where 
        /// hue indicates the orientation within a color wheel, while saturation and lightness indicate amplitudes along different axes.Hues are measured within 
        /// an RGB color space, which is similar to most RGB color spaces, except that shades of white and black are absent.
        ///
        /// Note:  while hue takes an RGB hex color value, it only uses this value to determine the basic color (its orientation around the color wheel), not its 
        /// saturation or lightness, which are indicated separately as percentage changes. For example, the hue for pure green may be defined as hue:0x00ff00 or 
        /// hue:0x000100 and both hues will be identical. (Both values point to pure green in the HSL color model.) RGB hue values which consist of equal parts 
        /// Red, Green and Blue — such as hue:0x0000000 (black) and hue:0xffffff (white) and all the pure shades of grey — do not indicate a hue whatsoever, as 
        /// none of those values indicate an orientation in the HSL coordinate space. To indicate black, white or grey, you must remove all saturation 
        /// (add a saturation:-100 operation) and adjust lightness instead.
        ///
        /// Additionally, when modifying existing features which already have a color scheme, changing a value such as hue does not change its 
        /// existing saturation or lightness.
        ///
        /// The following example displays a map of Brooklyn where local roads have been changed to bright green and the residential areas have been 
        /// changed to black(note that in this fully worked example the pipe separators have been properly URL encoded; see the note above)
        /// </remarks>
        public MapStyle[] Styles { get; set; }

        /// <summary>
        /// The map style type.
        /// </summary>
        public class MapStyle
        {
            /// <summary>
            /// Gets or sets a map consists of a set of features, such as roads or parks.
            /// feature:all (default) selects all features of the map.
            /// feature:road selects all roads on the map.
            /// feature:landscape selects all background landscapes on a map, which is often the area between roads, 
            /// for example.In cities, landscape usually consists of built - up areas.
            /// </summary>
            public string Feature { get; set; }

            /// <summary>
            /// Gets or sets elements within features are selected by declaring an element argument.
            /// element:all (default) selects all elements of that feature.
            /// element:geometry selects only geometric elements of that feature.
            /// element:labels selects only textual labels associated with that feature.
            /// </summary>
            public string Element { get; set; }

            /// <summary>
            /// Gets or sets weight (optional) specifies the thickness of the path in pixels. If no weight parameter 
            /// is set, the path will appear in its default thickness (5 pixels).
            /// </summary>
            public string Weight { get; set; }

            /// <summary>
            /// Gets or sets color (optional) specifies a color either as a 24-bit (example: color=0xFFFFCC) or 32-bit hexadecimal 
            /// value (example: color=0xFFFFCCFF), or from the set {black, brown, green, purple, yellow, blue, gray, orange, red, white}.
            /// </summary>
            public string Color { get; set; }

            /// <summary>
            /// Gets or sets fillcolor (optional) indicates both that the path marks off a polygonal area and specifies the 
            /// fill color to use as an overlay within that area. The set of locations following need not be a "closed" loop; 
            /// the Google Static Maps API server will automatically join the first and last points. Note, however, that any 
            /// stroke on the exterior of the filled area will not be closed unless you specifically provide the same beginning 
            /// and end location.
            /// </summary>
            public string FillColor { get; set; }

            /// <summary>
            /// Gets or sets hue (an RGB hex string of format 0xRRGGBB) indicates the basic color to apply to the selection.
            /// </summary>
            public string Hue { get; set; }

            /// <summary>
            /// Gets or sets lightness (a floating point value between -100 and 100) indicates the percentage change in brightness of the element. 
            /// Negative values increase darkness (where -100 specifies black) while positive values increase brightness (where +100 specifies white).
            /// </summary>
            public string Lightness { get; set; }

            /// <summary>
            /// Gets or sets saturation (a floating point value between -100 and 100) indicates the percentage change in
            /// intensity of the basic color to apply to the element.
            /// </summary>
            public string Saturation{ get; set; }

            /// <summary>
            /// Gets or sets gamma (a floating point value between 0.01 and 10.0, where 1.0 applies no correction) indicates the 
            /// amount of gamma correction to apply to the element. Gammas modify the lightness of hues in a non-linear fashion, 
            /// while unaffecting white or black values. Gammas are typically used to modify the contrast of multiple elements. 
            /// For example, you could modify the gamma to increase or decrease the contrast between the edges and interiors of 
            /// elements. Low gamma values (less than 1) increase contrast, while high values (greater than 1) decrease contrast.
            /// </summary>
            public string Gamma { get; set; }

            /// <summary>
            /// Gets or sets inverse lightness true simply inverts the existing lightness.
            /// </summary>
            public string InverseLightness { get; set; }

            /// <summary>
            /// Gets or sets visibility (on, off, or simplified) indicates whether and how the element appears on the map. 
            /// visibility: simplified indicates that the map should simplify the presentation of those elements as it sees fit. 
            /// (A simplified road structure may show fewer roads, for example.)
            /// </summary>
            public string Visibility { get; set; }
        }
    }
}
