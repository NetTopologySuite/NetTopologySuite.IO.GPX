using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a waypoint, point of interest, or named feature on a map.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_wptType">wptType</a>".
    /// </remarks>
    public sealed class GpxWaypoint
    {
        private readonly double elevationInMeters;

        private readonly DateTime timestampUtc;

        private readonly UncommonProperties uncommonProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// Provides just the essential information for indicating the location, and nothing else.
        /// </summary>
        /// <param name="longitude">
        /// The value for <see cref="Longitude"/>.
        /// </param>
        /// <param name="latitude">
        /// The value for <see cref="Latitude"/>.
        /// </param>
        /// <param name="elevationInMeters">
        /// The value for <see cref="ElevationInMeters"/>.
        /// </param>
        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude, double? elevationInMeters)
            : this(longitude, latitude, elevationInMeters, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="longitude">
        /// The value for <see cref="Longitude"/>.
        /// </param>
        /// <param name="latitude">
        /// The value for <see cref="Latitude"/>.
        /// </param>
        /// <param name="elevationInMeters">
        /// The value for <see cref="ElevationInMeters"/>.
        /// </param>
        /// <param name="timestampUtc">
        /// The value for <see cref="TimestampUtc"/>.
        /// </param>
        /// <param name="magneticVariation">
        /// The value for <see cref="MagneticVariation"/>.
        /// </param>
        /// <param name="geoidHeight">
        /// The value for <see cref="GeoidHeight"/>.
        /// </param>
        /// <param name="name">
        /// The value for <see cref="Name"/>.
        /// </param>
        /// <param name="comment">
        /// The value for <see cref="Comment"/>.
        /// </param>
        /// <param name="description">
        /// The value for <see cref="Description"/>.
        /// </param>
        /// <param name="source">
        /// The value for <see cref="Source"/>.
        /// </param>
        /// <param name="links">
        /// The value for <see cref="Links"/>.
        /// </param>
        /// <param name="symbolText">
        /// The value for <see cref="SymbolText"/>.
        /// </param>
        /// <param name="classification">
        /// The value for <see cref="Classification"/>.
        /// </param>
        /// <param name="fixKind">
        /// The value for <see cref="FixKind"/>.
        /// </param>
        /// <param name="numberOfSatellites">
        /// The value for <see cref="NumberOfSatellites"/>.
        /// </param>
        /// <param name="horizontalDilutionOfPrecision">
        /// The value for <see cref="HorizontalDilutionOfPrecision"/>.
        /// </param>
        /// <param name="verticalDilutionOfPrecision">
        /// The value for <see cref="VerticalDilutionOfPrecision"/>.
        /// </param>
        /// <param name="positionDilutionOfPrecision">
        /// The value for <see cref="PositionDilutionOfPrecision"/>.
        /// </param>
        /// <param name="secondsSinceLastDgpsUpdate">
        /// The value for <see cref="SecondsSinceLastDgpsUpdate"/>.
        /// </param>
        /// <param name="dgpsStationId">
        /// The value for <see cref="DgpsStationId"/>.
        /// </param>
        /// <param name="extensions">
        /// The value for <see cref="Extensions"/>.
        /// </param>
        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude, double? elevationInMeters, DateTime? timestampUtc, GpxDegrees? magneticVariation, double? geoidHeight, string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, string symbolText, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
            if (elevationInMeters is null)
            {
                this.elevationInMeters = double.NaN;
            }
            else
            {
                this.elevationInMeters = elevationInMeters.GetValueOrDefault();
                if (double.IsNaN(this.elevationInMeters))
                {
                    throw new ArgumentException("Must be a number", nameof(elevationInMeters));
                }
            }

            if (timestampUtc is null)
            {
                this.timestampUtc = new DateTime(0, DateTimeKind.Unspecified);
            }
            else
            {
                this.timestampUtc = timestampUtc.GetValueOrDefault();
                if (this.timestampUtc.Kind != DateTimeKind.Utc)
                {
                    throw new ArgumentException("Must be UTC", nameof(timestampUtc));
                }
            }

            this.Name = name;
            this.Description = description;
            this.SymbolText = symbolText;

            // only allocate space for these less commonly used properties if they're used.
            if (magneticVariation is null && geoidHeight is null && comment is null && source is null && links.IsDefaultOrEmpty && classification is null && fixKind is null && numberOfSatellites is null && horizontalDilutionOfPrecision is null && verticalDilutionOfPrecision is null && positionDilutionOfPrecision is null && secondsSinceLastDgpsUpdate is null && dgpsStationId is null && extensions is null)
            {
                return;
            }

            this.uncommonProperties = new UncommonProperties(magneticVariation, geoidHeight, comment, source, links, classification, fixKind, numberOfSatellites, horizontalDilutionOfPrecision, verticalDilutionOfPrecision, positionDilutionOfPrecision, secondsSinceLastDgpsUpdate, dgpsStationId, extensions);
        }

        /// <summary>
        /// Gets the longitude of this location, in decimal degrees (WGS84).
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "lon" attribute.
        /// </remarks>
        public GpxLongitude Longitude { get; }

        /// <summary>
        /// Gets the latitude of this location, in decimal degrees (WGS84).
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "lat" attribute.
        /// </remarks>
        public GpxLatitude Latitude { get; }

        /// <summary>
        /// Gets the elevation of this point, in meters.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "ele" element.
        /// </remarks>
        public double? ElevationInMeters => double.IsNaN(this.elevationInMeters) ? default(double?) : this.elevationInMeters;

        /// <summary>
        /// Gets the creation / modification timestamp of this location, in UTC time.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "time" element.
        /// </remarks>
        public DateTime? TimestampUtc => this.timestampUtc.Kind == DateTimeKind.Unspecified ? default(DateTime?) : this.timestampUtc;

        /// <summary>
        /// Gets the magnetic variation at this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "magvar" element.
        /// </remarks>
        public GpxDegrees? MagneticVariation => this.uncommonProperties?.MagneticVariation;

        /// <summary>
        /// Gets the height (in meters) of geoid (mean sea level) above WGS84 earth ellipsoid at
        /// this location, as defined in NMEA GGA message.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "geoidheight" element.
        /// </remarks>
        public double? GeoidHeight => this.uncommonProperties?.GeoidHeight;

        /// <summary>
        /// Gets the GPS name of this location, to be transferred to and from the GPS.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "name" element.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the GPS comment of this location, to be transferred to and from the GPS.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "cmt" element.
        /// </remarks>
        public string Comment => this.uncommonProperties?.Comment;

        /// <summary>
        /// Gets an additional text description for this location for the user (not the GPS).
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "desc" element.
        /// </remarks>
        public string Description { get; }

        /// <summary>
        /// Gets the source of the data, e.g., "Garmin eTrex", "USGS quad Boston North".
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "src" element.
        /// </remarks>
        public string Source => this.uncommonProperties?.Source;

        /// <summary>
        /// Gets links to additional information about this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" elements.
        /// </remarks>
        public ImmutableArray<GpxWebLink> Links => this.uncommonProperties?.Links ?? ImmutableArray<GpxWebLink>.Empty;

        /// <summary>
        /// Gets the text of the GPS symbol name for this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "sym" element.
        /// </remarks>
        public string SymbolText { get; }

        /// <summary>
        /// Gets the classification of this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "type" element.
        /// </remarks>
        public string Classification => this.uncommonProperties?.Classification;

        /// <summary>
        /// Gets the type of fix when this waypoint was generated.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "fix" element.
        /// </remarks>
        public GpxFixKind? FixKind => this.uncommonProperties?.FixKind;

        /// <summary>
        /// Gets the number of satellites used to calculate the GPS fix.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "sat" element.
        /// </remarks>
        public uint? NumberOfSatellites => this.uncommonProperties?.NumberOfSatellites;

        /// <summary>
        /// Gets the horizontal dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "hdop" element.
        /// </remarks>
        public double? HorizontalDilutionOfPrecision => this.uncommonProperties?.HorizontalDilutionOfPrecision;

        /// <summary>
        /// Gets the vertical dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "vdop" element.
        /// </remarks>
        public double? VerticalDilutionOfPrecision => this.uncommonProperties?.VerticalDilutionOfPrecision;

        /// <summary>
        /// Gets the position dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "pdop" element.
        /// </remarks>
        public double? PositionDilutionOfPrecision => this.uncommonProperties?.PositionDilutionOfPrecision;

        /// <summary>
        /// Gets the number of seconds since the last DGPS update when this waypoint was generated.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "ageofdgpsdata" element.
        /// </remarks>
        public double? SecondsSinceLastDgpsUpdate => this.uncommonProperties?.SecondsSinceLastDgpsUpdate;

        /// <summary>
        /// Gets the ID of the DGPS station used in differential correction.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "dgpsid" element.
        /// </remarks>
        public GpxDgpsStationId? DgpsStationId => this.uncommonProperties?.DgpsStationId;

        /// <summary>
        /// Gets arbitrary extension information associated with this waypoint.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions => this.uncommonProperties?.Extensions;

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Longitude), this.Longitude),
                                                                 (nameof(this.Latitude), this.Latitude),
                                                                 (nameof(this.ElevationInMeters), this.ElevationInMeters),
                                                                 (nameof(this.TimestampUtc), this.TimestampUtc),
                                                                 (nameof(this.MagneticVariation), this.MagneticVariation),
                                                                 (nameof(this.GeoidHeight), this.GeoidHeight),
                                                                 (nameof(this.Name), this.Name),
                                                                 (nameof(this.Comment), this.Comment),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Source), this.Source),
                                                                 (nameof(this.Links), Helpers.ListToString(this.Links)),
                                                                 (nameof(this.SymbolText), this.SymbolText),
                                                                 (nameof(this.Classification), this.Classification),
                                                                 (nameof(this.FixKind), this.FixKind),
                                                                 (nameof(this.NumberOfSatellites), this.NumberOfSatellites),
                                                                 (nameof(this.HorizontalDilutionOfPrecision), this.HorizontalDilutionOfPrecision),
                                                                 (nameof(this.VerticalDilutionOfPrecision), this.VerticalDilutionOfPrecision),
                                                                 (nameof(this.PositionDilutionOfPrecision), this.PositionDilutionOfPrecision),
                                                                 (nameof(this.SecondsSinceLastDgpsUpdate), this.SecondsSinceLastDgpsUpdate),
                                                                 (nameof(this.DgpsStationId), this.DgpsStationId),
                                                                 (nameof(this.Extensions), this.Extensions));

        internal static GpxWaypoint Load(XElement element, GpxReaderSettings settings, Func<IEnumerable<XElement>, object> extensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            var extensionsElement = element.GpxElement("extensions");
            return new GpxWaypoint(
                longitude: Helpers.ParseLongitude(element.Attribute("lon")?.Value) ?? throw new XmlException("waypoint must have lon attribute"),
                latitude: Helpers.ParseLatitude(element.Attribute("lat")?.Value) ?? throw new XmlException("waypoint must have lat attribute"),
                elevationInMeters: Helpers.ParseDouble(element.GpxElement("ele")?.Value),
                timestampUtc: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo),
                magneticVariation: Helpers.ParseDegrees(element.GpxElement("magvar")?.Value),
                geoidHeight: Helpers.ParseDouble(element.GpxElement("geoidheight")?.Value),
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                symbolText: element.GpxElement("sym")?.Value,
                classification: element.GpxElement("type")?.Value,
                fixKind: Helpers.ParseFixKind(element.GpxElement("fix")?.Value),
                numberOfSatellites: Helpers.ParseUInt32(element.GpxElement("sat")?.Value),
                horizontalDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("hdop")?.Value),
                verticalDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("vdop")?.Value),
                positionDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("pdop")?.Value),
                secondsSinceLastDgpsUpdate: Helpers.ParseDouble(element.GpxElement("ageofdgpsdata")?.Value),
                dgpsStationId: Helpers.ParseDgpsStationId(element.GpxElement("dgpsid")?.Value),
                extensions: extensionsElement is null ? null : extensionCallback(extensionsElement.Elements()));
        }

        internal void Save(XmlWriter writer, GpxWriterSettings settings, Func<object, IEnumerable<XElement>> extensionCallback)
        {
            writer.WriteAttributeString("lat", this.Latitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lon", this.Longitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteOptionalGpxElementValue("ele", this.ElevationInMeters);
            writer.WriteOptionalGpxElementValue("time", this.TimestampUtc);
            writer.WriteOptionalGpxElementValue("magvar", this.MagneticVariation);
            writer.WriteOptionalGpxElementValue("geoidheight", this.GeoidHeight);
            writer.WriteOptionalGpxElementValue("name", this.Name);
            writer.WriteOptionalGpxElementValue("cmt", this.Comment);
            writer.WriteOptionalGpxElementValue("desc", this.Description);
            writer.WriteOptionalGpxElementValue("src", this.Source);
            writer.WriteGpxElementValues("link", this.Links);
            writer.WriteOptionalGpxElementValue("sym", this.SymbolText);
            writer.WriteOptionalGpxElementValue("type", this.Classification);
            writer.WriteOptionalGpxElementValue("fix", this.FixKind);
            writer.WriteOptionalGpxElementValue("sat", this.NumberOfSatellites);
            writer.WriteOptionalGpxElementValue("hdop", this.HorizontalDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("vdop", this.VerticalDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("pdop", this.PositionDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("ageofdgpsdata", this.SecondsSinceLastDgpsUpdate);
            writer.WriteOptionalGpxElementValue("dgpsid", this.DgpsStationId);
            writer.WriteExtensions(this.Extensions, extensionCallback);
        }

        private sealed class UncommonProperties
        {
            public UncommonProperties(GpxDegrees? magneticVariation, double? geoidHeight, string comment, string source, ImmutableArray<GpxWebLink> links, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
            {
                this.MagneticVariation = magneticVariation;
                this.GeoidHeight = geoidHeight;
                this.Comment = comment;
                this.Source = source;
                this.Links = links.IsDefault ? ImmutableArray<GpxWebLink>.Empty : links;
                this.Classification = classification;
                this.FixKind = fixKind;
                this.NumberOfSatellites = numberOfSatellites;
                this.HorizontalDilutionOfPrecision = horizontalDilutionOfPrecision;
                this.VerticalDilutionOfPrecision = verticalDilutionOfPrecision;
                this.PositionDilutionOfPrecision = positionDilutionOfPrecision;
                this.SecondsSinceLastDgpsUpdate = secondsSinceLastDgpsUpdate;
                this.DgpsStationId = dgpsStationId;
                this.Extensions = extensions;
            }

            public GpxDegrees? MagneticVariation { get; }

            public double? GeoidHeight { get; }

            public string Comment { get; }

            public string Source { get; }

            public ImmutableArray<GpxWebLink> Links { get; }

            public string Classification { get; }

            public GpxFixKind? FixKind { get; }

            public uint? NumberOfSatellites { get; }

            public double? HorizontalDilutionOfPrecision { get; }

            public double? VerticalDilutionOfPrecision { get; }

            public double? PositionDilutionOfPrecision { get; }

            public double? SecondsSinceLastDgpsUpdate { get; }

            public GpxDgpsStationId? DgpsStationId { get; }

            public object Extensions { get; }
        }
    }
}
