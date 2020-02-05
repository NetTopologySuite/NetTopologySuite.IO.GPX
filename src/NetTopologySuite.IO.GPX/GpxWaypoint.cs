using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using NetTopologySuite.Geometries;

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
        private readonly double _elevationInMeters = double.NaN;

        private readonly DateTime _timestampUtc = new DateTime(0, DateTimeKind.Unspecified);

        private readonly UncommonProperties _uncommonProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="longitude">
        /// The value for <see cref="Longitude"/>.
        /// </param>
        /// <param name="latitude">
        /// The value for <see cref="Latitude"/>.
        /// </param>
        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude)
            : this(longitude, latitude, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="coordinate">
        /// A <see cref="Coordinate"/> to use to initialize <see cref="Longitude"/> and
        /// <see cref="Latitude"/>.
        /// </param>
        /// <remarks>
        /// <see cref="Coordinate.X"/> and <see cref="Coordinate.Y"/> are assumed to be WGS-84
        /// degrees, and <see cref="Coordinate.Z"/> is assumed to be an elevation in meters when it
        /// isn't equal to <see cref="Coordinate.NullOrdinate"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="coordinate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="coordinate"/>'s <see cref="Coordinate.X"/> value is not within the range of valid values for <see cref="GpxLongitude"/>,</description></item>
        /// <item><description><paramref name="coordinate"/>'s <see cref="Coordinate.Y"/> value is not within the range of valid values for <see cref="GpxLatitude"/>, or</description></item>
        /// <item><description><paramref name="coordinate"/>'s <see cref="Coordinate.Z"/> value <see cref="double.IsInfinity">is infinite</see></description></item>
        /// </list>
        /// </exception>
        public GpxWaypoint(Coordinate coordinate)
            : this(default, default)
        {
            if (coordinate is null)
            {
                throw new ArgumentNullException(nameof(coordinate));
            }

            if (!(Math.Abs(coordinate.X) <= 180))
            {
                throw new ArgumentException("X must be a valid WGS-84 longitude value, in degrees", nameof(coordinate));
            }

            if (!(Math.Abs(coordinate.Y) <= 90))
            {
                throw new ArgumentException("Y must be a valid WGS-84 latitude value, in degrees", nameof(coordinate));
            }

            if (double.IsInfinity(coordinate.Z))
            {
                throw new ArgumentException("Z must be either a finite value, in meters, or Coordinate.NullOrdinate if the coordinate is two-dimensional", nameof(coordinate));
            }

            double longitudeValue = coordinate.X;

            // +180 and -180 represent the same longitude value, so the GPX schema only allows -180.
            if (longitudeValue == 180)
            {
                longitudeValue = -180;
            }

            Longitude = new GpxLongitude(longitudeValue);
            Latitude = new GpxLatitude(coordinate.Y);
            if (!double.IsNaN(coordinate.Z))
            {
                _elevationInMeters = coordinate.Z;
            }
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="elevationInMeters"/> is <see cref="Nullable{T}.HasValue">non-null</see> and not a finite number.
        /// </exception>
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="elevationInMeters"/> is <see cref="Nullable{T}.HasValue">non-null</see> and not a finite number, or</description></item>
        /// <item><description><paramref name="timestampUtc"/> is <see cref="Nullable{T}.HasValue">non-null</see> and its <see cref="DateTime.Kind"/> is not <see cref="DateTimeKind.Utc"/>.</description></item>
        /// </list>
        /// </exception>
        public GpxWaypoint(GpxLongitude longitude, GpxLatitude latitude, double? elevationInMeters, DateTime? timestampUtc, GpxDegrees? magneticVariation, double? geoidHeight, string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, string symbolText, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
        {
            Longitude = longitude;
            Latitude = latitude;
            if (!(elevationInMeters is null))
            {
                _elevationInMeters = elevationInMeters.GetValueOrDefault();
                if (!_elevationInMeters.IsFinite())
                {
                    throw new ArgumentOutOfRangeException(nameof(elevationInMeters), elevationInMeters, "Must be a finite number");
                }
            }

            if (!(timestampUtc is null))
            {
                _timestampUtc = timestampUtc.GetValueOrDefault();
                if (_timestampUtc.Kind != DateTimeKind.Utc)
                {
                    throw new ArgumentOutOfRangeException(nameof(timestampUtc), timestampUtc, "Must be UTC");
                }
            }

            Name = name;
            Description = description;
            SymbolText = symbolText;

            // only allocate space for these less commonly used properties if they're used.
            if (magneticVariation is null && geoidHeight is null && comment is null && source is null && links.IsDefaultOrEmpty && classification is null && fixKind is null && numberOfSatellites is null && horizontalDilutionOfPrecision is null && verticalDilutionOfPrecision is null && positionDilutionOfPrecision is null && secondsSinceLastDgpsUpdate is null && dgpsStationId is null && extensions is null)
            {
                return;
            }

            _uncommonProperties = new UncommonProperties(magneticVariation, geoidHeight, comment, source, links, classification, fixKind, numberOfSatellites, horizontalDilutionOfPrecision, verticalDilutionOfPrecision, positionDilutionOfPrecision, secondsSinceLastDgpsUpdate, dgpsStationId, extensions);
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
        public double? ElevationInMeters => double.IsNaN(_elevationInMeters) ? default(double?) : _elevationInMeters;

        /// <summary>
        /// Gets the creation / modification timestamp of this location, in UTC time.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "time" element.
        /// </remarks>
        public DateTime? TimestampUtc => _timestampUtc.Kind == DateTimeKind.Unspecified ? default(DateTime?) : _timestampUtc;

        /// <summary>
        /// Gets the magnetic variation at this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "magvar" element.
        /// </remarks>
        public GpxDegrees? MagneticVariation => _uncommonProperties?.MagneticVariation;

        /// <summary>
        /// Gets the height (in meters) of geoid (mean sea level) above WGS84 earth ellipsoid at
        /// this location, as defined in NMEA GGA message.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "geoidheight" element.
        /// </remarks>
        public double? GeoidHeight => _uncommonProperties?.GeoidHeight;

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
        public string Comment => _uncommonProperties?.Comment;

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
        public string Source => _uncommonProperties?.Source;

        /// <summary>
        /// Gets links to additional information about this location.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" elements.
        /// </remarks>
        public ImmutableArray<GpxWebLink> Links => _uncommonProperties?.Links ?? ImmutableArray<GpxWebLink>.Empty;

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
        public string Classification => _uncommonProperties?.Classification;

        /// <summary>
        /// Gets the type of fix when this waypoint was generated.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "fix" element.
        /// </remarks>
        public GpxFixKind? FixKind => _uncommonProperties?.FixKind;

        /// <summary>
        /// Gets the number of satellites used to calculate the GPS fix.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "sat" element.
        /// </remarks>
        public uint? NumberOfSatellites => _uncommonProperties?.NumberOfSatellites;

        /// <summary>
        /// Gets the horizontal dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "hdop" element.
        /// </remarks>
        public double? HorizontalDilutionOfPrecision => _uncommonProperties?.HorizontalDilutionOfPrecision;

        /// <summary>
        /// Gets the vertical dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "vdop" element.
        /// </remarks>
        public double? VerticalDilutionOfPrecision => _uncommonProperties?.VerticalDilutionOfPrecision;

        /// <summary>
        /// Gets the position dilution of precision.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "pdop" element.
        /// </remarks>
        public double? PositionDilutionOfPrecision => _uncommonProperties?.PositionDilutionOfPrecision;

        /// <summary>
        /// Gets the number of seconds since the last DGPS update when this waypoint was generated.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "ageofdgpsdata" element.
        /// </remarks>
        public double? SecondsSinceLastDgpsUpdate => _uncommonProperties?.SecondsSinceLastDgpsUpdate;

        /// <summary>
        /// Gets the ID of the DGPS station used in differential correction.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "dgpsid" element.
        /// </remarks>
        public GpxDgpsStationId? DgpsStationId => _uncommonProperties?.DgpsStationId;

        /// <summary>
        /// Gets arbitrary extension information associated with this waypoint.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions => _uncommonProperties?.Extensions;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxWaypoint other &&
                                                   Longitude == other.Longitude &&
                                                   Latitude == other.Latitude &&
                                                   _elevationInMeters == other._elevationInMeters &&
                                                   _timestampUtc == other._timestampUtc &&
                                                   Name == other.Name &&
                                                   Description == other.Description &&
                                                   SymbolText == other.SymbolText &&
                                                   Equals(_uncommonProperties, other._uncommonProperties);

        /// <inheritdoc />
        public override int GetHashCode() => (Longitude, Latitude, _elevationInMeters, _timestampUtc, Name, Description, SymbolText, _uncommonProperties).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(Longitude), Longitude),
                                                                 (nameof(Latitude), Latitude),
                                                                 (nameof(ElevationInMeters), ElevationInMeters),
                                                                 (nameof(TimestampUtc), TimestampUtc),
                                                                 (nameof(MagneticVariation), MagneticVariation),
                                                                 (nameof(GeoidHeight), GeoidHeight),
                                                                 (nameof(Name), Name),
                                                                 (nameof(Comment), Comment),
                                                                 (nameof(Description), Description),
                                                                 (nameof(Source), Source),
                                                                 (nameof(Links), Helpers.ListToString(Links)),
                                                                 (nameof(SymbolText), SymbolText),
                                                                 (nameof(Classification), Classification),
                                                                 (nameof(FixKind), FixKind),
                                                                 (nameof(NumberOfSatellites), NumberOfSatellites),
                                                                 (nameof(HorizontalDilutionOfPrecision), HorizontalDilutionOfPrecision),
                                                                 (nameof(VerticalDilutionOfPrecision), VerticalDilutionOfPrecision),
                                                                 (nameof(PositionDilutionOfPrecision), PositionDilutionOfPrecision),
                                                                 (nameof(SecondsSinceLastDgpsUpdate), SecondsSinceLastDgpsUpdate),
                                                                 (nameof(DgpsStationId), DgpsStationId),
                                                                 (nameof(Extensions), Extensions));

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Longitude"/> replaced by the given value.
        /// </summary>
        /// <param name="longitude">
        /// The new value for <see cref="Longitude"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Longitude"/> value set to <paramref name="longitude"/>.
        /// </returns>
        public GpxWaypoint WithLongitude(GpxLongitude longitude) => new GpxWaypoint(longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Latitude"/> replaced by the given value.
        /// </summary>
        /// <param name="latitude">
        /// The new value for <see cref="Latitude"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Latitude"/> value set to <paramref name="latitude"/>.
        /// </returns>
        public GpxWaypoint WithLatitude(GpxLatitude latitude) => new GpxWaypoint(Longitude, latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="ElevationInMeters"/> replaced by the given value.
        /// </summary>
        /// <param name="elevationInMeters">
        /// The new value for <see cref="ElevationInMeters"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="ElevationInMeters"/> value set to <paramref name="elevationInMeters"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="elevationInMeters"/> is <see cref="Nullable{T}.HasValue">non-null</see> and <see cref="double.IsNaN">not a number</see>.
        /// </exception>
        public GpxWaypoint WithElevationInMeters(double? elevationInMeters) => new GpxWaypoint(Longitude, Latitude, elevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="TimestampUtc"/> replaced by the given value.
        /// </summary>
        /// <param name="timestampUtc">
        /// The new value for <see cref="TimestampUtc"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="TimestampUtc"/> value set to <paramref name="timestampUtc"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="timestampUtc"/> is <see cref="Nullable{T}.HasValue">non-null</see> and its <see cref="DateTime.Kind"/> is not <see cref="DateTimeKind.Utc"/>.
        /// </exception>
        public GpxWaypoint WithTimestampUtc(DateTime? timestampUtc) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, timestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="MagneticVariation"/> replaced by the given value.
        /// </summary>
        /// <param name="magneticVariation">
        /// The new value for <see cref="MagneticVariation"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="MagneticVariation"/> value set to <paramref name="magneticVariation"/>.
        /// </returns>
        public GpxWaypoint WithMagneticVariation(GpxDegrees? magneticVariation) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, magneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="GeoidHeight"/> replaced by the given value.
        /// </summary>
        /// <param name="geoidHeight">
        /// The new value for <see cref="GeoidHeight"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="GeoidHeight"/> value set to <paramref name="geoidHeight"/>.
        /// </returns>
        public GpxWaypoint WithGeoidHeight(double? geoidHeight) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, geoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Name"/> replaced by the given value.
        /// </summary>
        /// <param name="name">
        /// The new value for <see cref="Name"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Name"/> value set to <paramref name="name"/>.
        /// </returns>
        public GpxWaypoint WithName(string name) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Comment"/> replaced by the given value.
        /// </summary>
        /// <param name="comment">
        /// The new value for <see cref="Comment"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Comment"/> value set to <paramref name="comment"/>.
        /// </returns>
        public GpxWaypoint WithComment(string comment) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Description"/> replaced by the given value.
        /// </summary>
        /// <param name="description">
        /// The new value for <see cref="Description"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Description"/> value set to <paramref name="description"/>.
        /// </returns>
        public GpxWaypoint WithDescription(string description) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Source"/> replaced by the given value.
        /// </summary>
        /// <param name="source">
        /// The new value for <see cref="Source"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Source"/> value set to <paramref name="source"/>.
        /// </returns>
        public GpxWaypoint WithSource(string source) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Links"/> replaced by the given value.
        /// </summary>
        /// <param name="links">
        /// The new value for <see cref="Links"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Links"/> value set to <paramref name="links"/>.
        /// </returns>
        public GpxWaypoint WithLinks(ImmutableArray<GpxWebLink> links) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="SymbolText"/> replaced by the given value.
        /// </summary>
        /// <param name="symbolText">
        /// The new value for <see cref="SymbolText"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="SymbolText"/> value set to <paramref name="symbolText"/>.
        /// </returns>
        public GpxWaypoint WithSymbolText(string symbolText) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, symbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Classification"/> replaced by the given value.
        /// </summary>
        /// <param name="classification">
        /// The new value for <see cref="Classification"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Classification"/> value set to <paramref name="classification"/>.
        /// </returns>
        public GpxWaypoint WithClassification(string classification) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="FixKind"/> replaced by the given value.
        /// </summary>
        /// <param name="fixKind">
        /// The new value for <see cref="FixKind"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="FixKind"/> value set to <paramref name="fixKind"/>.
        /// </returns>
        public GpxWaypoint WithFixKind(GpxFixKind? fixKind) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, fixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="NumberOfSatellites"/> replaced by the given value.
        /// </summary>
        /// <param name="numberOfSatellites">
        /// The new value for <see cref="NumberOfSatellites"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="NumberOfSatellites"/> value set to <paramref name="numberOfSatellites"/>.
        /// </returns>
        public GpxWaypoint WithNumberOfSatellites(uint? numberOfSatellites) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, numberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="HorizontalDilutionOfPrecision"/> replaced by the given value.
        /// </summary>
        /// <param name="horizontalDilutionOfPrecision">
        /// The new value for <see cref="HorizontalDilutionOfPrecision"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="HorizontalDilutionOfPrecision"/> value set to <paramref name="horizontalDilutionOfPrecision"/>.
        /// </returns>
        public GpxWaypoint WithHorizontalDilutionOfPrecision(double? horizontalDilutionOfPrecision) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, horizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="VerticalDilutionOfPrecision"/> replaced by the given value.
        /// </summary>
        /// <param name="verticalDilutionOfPrecision">
        /// The new value for <see cref="VerticalDilutionOfPrecision"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="VerticalDilutionOfPrecision"/> value set to <paramref name="verticalDilutionOfPrecision"/>.
        /// </returns>
        public GpxWaypoint WithVerticalDilutionOfPrecision(double? verticalDilutionOfPrecision) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, verticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="PositionDilutionOfPrecision"/> replaced by the given value.
        /// </summary>
        /// <param name="positionDilutionOfPrecision">
        /// The new value for <see cref="PositionDilutionOfPrecision"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="PositionDilutionOfPrecision"/> value set to <paramref name="positionDilutionOfPrecision"/>.
        /// </returns>
        public GpxWaypoint WithPositionDilutionOfPrecision(double? positionDilutionOfPrecision) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, positionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="SecondsSinceLastDgpsUpdate"/> replaced by the given value.
        /// </summary>
        /// <param name="secondsSinceLastDgpsUpdate">
        /// The new value for <see cref="SecondsSinceLastDgpsUpdate"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="SecondsSinceLastDgpsUpdate"/> value set to <paramref name="secondsSinceLastDgpsUpdate"/>.
        /// </returns>
        public GpxWaypoint WithSecondsSinceLastDgpsUpdate(double? secondsSinceLastDgpsUpdate) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, secondsSinceLastDgpsUpdate, DgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="DgpsStationId"/> replaced by the given value.
        /// </summary>
        /// <param name="dgpsStationId">
        /// The new value for <see cref="DgpsStationId"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="DgpsStationId"/> value set to <paramref name="dgpsStationId"/>.
        /// </returns>
        public GpxWaypoint WithDgpsStationId(GpxDgpsStationId? dgpsStationId) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, dgpsStationId, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWaypoint"/> as a copy of this instance, but with
        /// <see cref="Extensions"/> replaced by the given value.
        /// </summary>
        /// <param name="extensions">
        /// The new value for <see cref="Extensions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWaypoint"/> instance that's a copy of the current instance, but
        /// with its <see cref="Extensions"/> value set to <paramref name="extensions"/>.
        /// </returns>
        public GpxWaypoint WithExtensions(object extensions) => new GpxWaypoint(Longitude, Latitude, ElevationInMeters, TimestampUtc, MagneticVariation, GeoidHeight, Name, Comment, Description, Source, Links, SymbolText, Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, extensions);

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
                timestampUtc: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo, settings.IgnoreBadDateTime),
                magneticVariation: Helpers.ParseDegrees(element.GpxElement("magvar")?.Value),
                geoidHeight: Helpers.ParseDouble(element.GpxElement("geoidheight")?.Value),
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(el => GpxWebLink.Load(el, settings.BuildWebLinksForVeryLongUriValues))),
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
            writer.WriteAttributeString("lat", Latitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lon", Longitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteOptionalGpxElementValue("ele", ElevationInMeters);
            writer.WriteOptionalGpxElementValue("time", TimestampUtc, settings.TimeZoneInfo);
            writer.WriteOptionalGpxElementValue("magvar", MagneticVariation);
            writer.WriteOptionalGpxElementValue("geoidheight", GeoidHeight);
            writer.WriteOptionalGpxElementValue("name", Name);
            writer.WriteOptionalGpxElementValue("cmt", Comment);
            writer.WriteOptionalGpxElementValue("desc", Description);
            writer.WriteOptionalGpxElementValue("src", Source);
            writer.WriteGpxElementValues("link", Links);
            writer.WriteOptionalGpxElementValue("sym", SymbolText);
            writer.WriteOptionalGpxElementValue("type", Classification);
            writer.WriteOptionalGpxElementValue("fix", FixKind);
            writer.WriteOptionalGpxElementValue("sat", NumberOfSatellites);
            writer.WriteOptionalGpxElementValue("hdop", HorizontalDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("vdop", VerticalDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("pdop", PositionDilutionOfPrecision);
            writer.WriteOptionalGpxElementValue("ageofdgpsdata", SecondsSinceLastDgpsUpdate);
            writer.WriteOptionalGpxElementValue("dgpsid", DgpsStationId);
            writer.WriteExtensions(Extensions, extensionCallback);
        }

        private sealed class UncommonProperties
        {
            public UncommonProperties(GpxDegrees? magneticVariation, double? geoidHeight, string comment, string source, ImmutableArray<GpxWebLink> links, string classification, GpxFixKind? fixKind, uint? numberOfSatellites, double? horizontalDilutionOfPrecision, double? verticalDilutionOfPrecision, double? positionDilutionOfPrecision, double? secondsSinceLastDgpsUpdate, GpxDgpsStationId? dgpsStationId, object extensions)
            {
                MagneticVariation = magneticVariation;
                GeoidHeight = geoidHeight;
                Comment = comment;
                Source = source;
                Links = links.IsDefault ? ImmutableArray<GpxWebLink>.Empty : links;
                Classification = classification;
                FixKind = fixKind;
                NumberOfSatellites = numberOfSatellites;
                HorizontalDilutionOfPrecision = horizontalDilutionOfPrecision;
                VerticalDilutionOfPrecision = verticalDilutionOfPrecision;
                PositionDilutionOfPrecision = positionDilutionOfPrecision;
                SecondsSinceLastDgpsUpdate = secondsSinceLastDgpsUpdate;
                DgpsStationId = dgpsStationId;
                Extensions = extensions;
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

            public override bool Equals(object obj) => obj is UncommonProperties other &&
                                                       MagneticVariation == other.MagneticVariation &&
                                                       GeoidHeight == other.GeoidHeight &&
                                                       Comment == other.Comment &&
                                                       Source == other.Source &&
                                                       Links.ListEquals(other.Links) &&
                                                       Classification == other.Classification &&
                                                       FixKind == other.FixKind &&
                                                       NumberOfSatellites == other.NumberOfSatellites &&
                                                       HorizontalDilutionOfPrecision == other.HorizontalDilutionOfPrecision &&
                                                       VerticalDilutionOfPrecision == other.VerticalDilutionOfPrecision &&
                                                       PositionDilutionOfPrecision == other.PositionDilutionOfPrecision &&
                                                       SecondsSinceLastDgpsUpdate == other.SecondsSinceLastDgpsUpdate &&
                                                       DgpsStationId == other.DgpsStationId &&
                                                       Equals(Extensions, other.Extensions);

            public override int GetHashCode() => (MagneticVariation, GeoidHeight, Comment, Source, Links.ListToHashCode(), Classification, FixKind, NumberOfSatellites, HorizontalDilutionOfPrecision, VerticalDilutionOfPrecision, PositionDilutionOfPrecision, SecondsSinceLastDgpsUpdate, DgpsStationId, Extensions).GetHashCode();
        }
    }
}
