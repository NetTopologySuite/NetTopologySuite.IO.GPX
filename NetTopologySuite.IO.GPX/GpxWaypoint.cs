using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxWaypoint
    {
        private readonly double elevationInMeters;

        private readonly DateTime timestampUtc;

        private readonly UncommonProperties uncommonProperties;

        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude, double? elevationInMeters, DateTime? timestampUtc, string name, string description, string symbolText)
            : this(longitude, latitude, elevationInMeters, timestampUtc, name, description, symbolText, default, default, default, default, default, default, default, default, default, default, default, default, default, default)
        {
        }

        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude, double? elevationInMeters, DateTime? timestampUtc, string name, string description, string symbolText, GpxDegrees? magneticVariation, double? geoidHeight, string comment, string source, ImmutableArray<GpxWebLink> links, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
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

        public GpxLongitude Longitude { get; }

        public GpxLatitude Latitude { get; }

        public double? ElevationInMeters => double.IsNaN(this.elevationInMeters) ? default(double?) : this.elevationInMeters;

        public DateTime? TimestampUtc => this.timestampUtc.Kind == DateTimeKind.Unspecified ? default(DateTime?) : this.timestampUtc;

        public string Name { get; }

        public string Description { get; }

        public string SymbolText { get; }

        public GpxDegrees? MagneticVariation => this.uncommonProperties?.MagneticVariation;

        public double? GeoidHeight => this.uncommonProperties?.GeoidHeight;

        public string Comment => this.uncommonProperties?.Comment;

        public string Source => this.uncommonProperties?.Source;

        public ImmutableArray<GpxWebLink> Links => this.uncommonProperties?.Links ?? ImmutableArray<GpxWebLink>.Empty;

        public string Classification => this.uncommonProperties?.Classification;

        public GpxFixKind? FixKind => this.uncommonProperties?.FixKind;

        public uint? NumberOfSatellites => this.uncommonProperties?.NumberOfSatellites;

        public double? HorizontalDilutionOfPrecision => this.uncommonProperties?.HorizontalDilutionOfPrecision;

        public double? VerticalDilutionOfPrecision => this.uncommonProperties?.VerticalDilutionOfPrecision;

        public double? PositionDilutionOfPrecision => this.uncommonProperties?.PositionDilutionOfPrecision;

        public double? SecondsSinceLastDgpsUpdate => this.uncommonProperties?.SecondsSinceLastDgpsUpdate;

        public GpxDgpsStationId? DgpsStationId => this.uncommonProperties?.DgpsStationId;

        public object Extensions => this.uncommonProperties?.Extensions;

        public static GpxWaypoint Load(XElement element, GpxReaderSettings settings, Func<XElement, object> extensionCallback)
        {
            return LoadNoValidation(element,
                                    settings ?? throw new ArgumentNullException(nameof(settings)),
                                    extensionCallback ?? throw new ArgumentNullException(nameof(extensionCallback)));
        }

        internal static GpxWaypoint LoadNoValidation(XElement element, GpxReaderSettings settings, Func<XElement, object> extensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxWaypoint(
                longitude: Helpers.ParseLongitude(element.GpxAttribute("lon")?.Value) ?? throw new XmlException("waypoint must have lon attribute"),
                latitude: Helpers.ParseLatitude(element.GpxAttribute("lat")?.Value) ?? throw new XmlException("waypoint must have lat attribute"),
                elevationInMeters: Helpers.ParseDouble(element.GpxElement("ele")?.Value),
                timestampUtc: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo),
                name: element.GpxElement("name")?.Value,
                description: element.GpxElement("desc")?.Value,
                symbolText: element.GpxElement("sym")?.Value,
                magneticVariation: Helpers.ParseDegrees(element.GpxElement("magvar")?.Value),
                geoidHeight: Helpers.ParseDouble(element.GpxElement("geoidheight")?.Value),
                comment: element.GpxElement("cmt")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                classification: element.GpxElement("type")?.Value,
                fixKind: Helpers.ParseFixKind(element.GpxElement("fix")?.Value),
                numberOfSatellites: Helpers.ParseUInt32(element.GpxElement("sat")?.Value),
                horizontalDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("hdop")?.Value),
                verticalDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("vdop")?.Value),
                positionDilutionOfPrecision: Helpers.ParseDouble(element.GpxElement("pdop")?.Value),
                secondsSinceLastDgpsUpdate: Helpers.ParseDouble(element.GpxElement("ageofdgpsdata")?.Value),
                dgpsStationId: Helpers.ParseDgpsStationId(element.GpxElement("dgpsid")?.Value),
                extensions: extensionCallback(element.GpxElement("extensions")));
        }

        private sealed class UncommonProperties
        {
            public UncommonProperties(GpxDegrees? magneticVariation, double? geoidHeight, string comment, string source, ImmutableArray<GpxWebLink> links, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
            {
                this.MagneticVariation = magneticVariation;
                this.GeoidHeight = geoidHeight;
                this.Comment = comment;
                this.Source = source;
                this.Links = links;
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
