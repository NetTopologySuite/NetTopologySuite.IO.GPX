using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using GeoAPI;
using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Contains helper methods to convert back and forth between <see cref="IFeature"/> instances
    /// and the corresponding GPX data object.
    /// </summary>
    public static class NetTopologySuiteGpxFeatureConverter
    {
        /// <summary>
        /// Turns an <see cref="IPoint"/> <see cref="IFeature"/> into the <see cref="GpxWaypoint"/>
        /// instance that's equivalent to it.
        /// </summary>
        /// <param name="feature">
        /// The <see cref="IFeature"/> to convert.
        /// </param>
        /// <returns>
        /// A more-or-less equivalent <see cref="GpxWaypoint"/> to <paramref name="feature"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="feature"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown by <see cref="GpxLongitude(double)"/> or <see cref="GpxLatitude(double)"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="feature"/>'s <see cref="IFeature.Geometry"/> is not an
        /// instance of <see cref="IPoint"/>.
        /// </exception>
        /// <remarks>
        /// Values from <see cref="IFeature.Attributes"/> with keys whose names match the names of
        /// properties of <see cref="GpxWaypoint"/> will be used to populate those corresponding
        /// property values, with a few exceptions:
        /// <list type="bullet">
        /// <item><description><see cref="GpxWaypoint.Longitude"/> comes from <see cref="IPoint.X"/>,</description></item>
        /// <item><description><see cref="GpxWaypoint.Latitude"/> comes from <see cref="IPoint.Y"/>, and</description></item>
        /// <item><description><see cref="GpxWaypoint.ElevationInMeters"/> comes from <see cref="IPoint.Z"/> (when it's set)</description></item>
        /// </list>
        /// </remarks>
        public static GpxWaypoint ToGpxWaypoint(IFeature feature)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (!(feature.Geometry is IPoint wpt))
            {
                throw new ArgumentException("Geometry must be IPoint.", nameof(feature));
            }

            var attributes = feature.Attributes;
            return new GpxWaypoint(
                longitude: new GpxLongitude(wpt.X),
                latitude: new GpxLatitude(wpt.Y),
                elevationInMeters: double.IsNaN(wpt.Z) ? default(double?) : wpt.Z,
                timestampUtc: (DateTime?)attributes?.GetOptionalValue(nameof(GpxWaypoint.TimestampUtc)),
                magneticVariation: (GpxDegrees?)attributes?.GetOptionalValue(nameof(GpxWaypoint.MagneticVariation)),
                geoidHeight: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.GeoidHeight)),
                name: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Name)),
                comment: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Comment)),
                description: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Description)),
                source: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Source)),
                links: attributes?.GetOptionalValue(nameof(GpxWaypoint.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
                symbolText: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.SymbolText)),
                classification: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Classification)),
                fixKind: (GpxFixKind?)attributes?.GetOptionalValue(nameof(GpxWaypoint.FixKind)),
                numberOfSatellites: (uint?)attributes?.GetOptionalValue(nameof(GpxWaypoint.NumberOfSatellites)),
                horizontalDilutionOfPrecision: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.HorizontalDilutionOfPrecision)),
                verticalDilutionOfPrecision: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.VerticalDilutionOfPrecision)),
                positionDilutionOfPrecision: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.PositionDilutionOfPrecision)),
                secondsSinceLastDgpsUpdate: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.SecondsSinceLastDgpsUpdate)),
                dgpsStationId: (GpxDgpsStationId?)attributes?.GetOptionalValue(nameof(GpxWaypoint.DgpsStationId)),
                extensions: attributes?.GetOptionalValue(nameof(GpxWaypoint.Extensions)));
        }

        /// <summary>
        /// Turns an <see cref="ILineString"/> <see cref="IFeature"/> into the
        /// <see cref="GpxRoute"/> instance that's equivalent to it.
        /// </summary>
        /// <param name="feature">
        /// The <see cref="IFeature"/> to convert.
        /// </param>
        /// <returns>
        /// A more-or-less equivalent <see cref="GpxRoute"/> to <paramref name="feature"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="feature"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown by <see cref="GpxLongitude(double)"/> or <see cref="GpxLatitude(double)"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="feature"/>'s <see cref="IFeature.Geometry"/> is not an
        /// instance of <see cref="ILineString"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// Values from <see cref="IFeature.Attributes"/> with keys whose names match the names of
        /// properties of <see cref="GpxRoute"/> will be used to populate those corresponding
        /// property values.
        /// </para>
        /// <para>
        /// When the "Waypoints" attribute is populated in <see cref="IFeature.Attributes"/>, its
        /// values will be used for <see cref="GpxRoute.Waypoints"/>, and the
        /// <see cref="IFeature.Geometry"/> will be ignored except to validate that it is, in fact,
        /// <see cref="ILineString"/>.  Otherwise, waypoints will be built with only values for:
        /// <list type="bullet">
        /// <item><description><see cref="GpxWaypoint.Longitude"/>,</description></item>
        /// <item><description><see cref="GpxWaypoint.Latitude"/>, and</description></item>
        /// <item><description><see cref="GpxWaypoint.ElevationInMeters"/> (where <see cref="IPoint.Z"/> is set).</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static GpxRoute ToGpxRoute(IFeature feature)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (!(feature.Geometry is ILineString rte))
            {
                throw new ArgumentException("Geometry must be ILineString.", nameof(feature));
            }

            var attributes = feature.Attributes;
            return new GpxRoute(
                name: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Name)),
                comment: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Comment)),
                description: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Description)),
                source: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Source)),
                links: attributes?.GetOptionalValue(nameof(GpxRoute.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
                number: (uint?)attributes?.GetOptionalValue(nameof(GpxRoute.Number)),
                classification: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Classification)),
                extensions: attributes?.GetOptionalValue(nameof(GpxRoute.Extensions)),
                waypoints: new ImmutableGpxWaypointTable(((IEnumerable<GpxWaypoint>)attributes?.GetOptionalValue(nameof(GpxRoute.Waypoints))) ?? GenerateWaypoints(rte)));
        }

        /// <summary>
        /// Turns an <see cref="IMultiLineString"/> <see cref="IFeature"/> into the
        /// <see cref="GpxTrack"/> instance that's equivalent to it.
        /// </summary>
        /// <param name="feature">
        /// The <see cref="IFeature"/> to convert.
        /// </param>
        /// <returns>
        /// A more-or-less equivalent <see cref="GpxTrack"/> to <paramref name="feature"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="feature"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown by <see cref="GpxLongitude(double)"/> or <see cref="GpxLatitude(double)"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="feature"/>'s <see cref="IFeature.Geometry"/> is not an
        /// instance of <see cref="IMultiLineString"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// Values from <see cref="IFeature.Attributes"/> with keys whose names match the names of
        /// properties of <see cref="GpxTrack"/> will be used to populate those corresponding
        /// property values.
        /// </para>
        /// <para>
        /// When the "Segments" attribute is populated in <see cref="IFeature.Attributes"/>, its
        /// values will be used for <see cref="GpxTrack.Segments"/>, and the
        /// <see cref="IFeature.Geometry"/> will be ignored except to validate that it is, in fact,
        /// <see cref="IMultiLineString"/>.  Otherwise, segments will be built without anything for
        /// <see cref="GpxTrackSegment.Extensions"/>, and whose waypoints only have these set:
        /// <list type="bullet">
        /// <item><description><see cref="GpxWaypoint.Longitude"/>,</description></item>
        /// <item><description><see cref="GpxWaypoint.Latitude"/>, and</description></item>
        /// <item><description><see cref="GpxWaypoint.ElevationInMeters"/> (if <see cref="ICoordinateSequence.Ordinates"/> has the <see cref="Ordinates.Z"/> flag).</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static GpxTrack ToGpxTrack(IFeature feature)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (!(feature.Geometry is IMultiLineString trk))
            {
                throw new ArgumentException("Geometry must be IMultiLineString.", nameof(feature));
            }

            var attributes = feature.Attributes;
            var trackSegments = (IEnumerable<GpxTrackSegment>)attributes?.GetOptionalValue(nameof(GpxTrack.Segments));

            return new GpxTrack(
                name: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Name)),
                comment: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Comment)),
                description: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Description)),
                source: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Source)),
                links: attributes?.GetOptionalValue(nameof(GpxTrack.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
                number: (uint?)attributes?.GetOptionalValue(nameof(GpxTrack.Number)),
                classification: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Classification)),
                extensions: attributes?.GetOptionalValue(nameof(GpxTrack.Extensions)),
                segments: ImmutableArray.CreateRange(trackSegments ?? GenerateTrackSegments(trk)));
        }

        /// <summary>
        /// Creates an <see cref="IPoint"/> <see cref="Feature"/> that contains the same data stored
        /// in a given <see cref="GpxWaypoint"/>.
        /// </summary>
        /// <param name="waypoint">
        /// The <see cref="GpxWaypoint"/> source.
        /// </param>
        /// <param name="geometryFactory">
        /// The <see cref="IGeometryFactory"/> to use to create the <see cref="IPoint"/>, or
        /// <see langword="null"/> to create a new one to use on-the-fly using the current
        /// <see cref="GeometryServiceProvider"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Feature"/> that contains the same data as <paramref name="waypoint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="waypoint"/> is <see langword="null"/>
        /// </exception>
        /// <remarks>
        /// The values of <see cref="Feature.Attributes"/> will be populated with the values of the
        /// <see cref="GpxWaypoint"/> properties, even when <see langword="null"/>, with exceptions:
        /// <list type="bullet">
        /// <item><description><see cref="GpxWaypoint.Longitude"/> goes to <see cref="IPoint.X"/> instead,</description></item>
        /// <item><description><see cref="GpxWaypoint.Latitude"/> goes to <see cref="IPoint.Y"/> instead, and</description></item>
        /// <item><description><see cref="GpxWaypoint.ElevationInMeters"/> goes to <see cref="IPoint.Z"/> instead (when it's set)</description></item>
        /// </list>
        /// </remarks>
        public static Feature ToFeature(GpxWaypoint waypoint, IGeometryFactory geometryFactory)
        {
            if (waypoint is null)
            {
                throw new ArgumentNullException(nameof(waypoint));
            }

            if (geometryFactory is null)
            {
                geometryFactory = GeometryServiceProvider.Instance.CreateGeometryFactory();
            }

            // a waypoint all on its own is an IPoint feature.
            var coord = new Coordinate(waypoint.Longitude, waypoint.Latitude, waypoint.ElevationInMeters ?? Coordinate.NullOrdinate);
            var point = geometryFactory.CreatePoint(coord);
            var attributes = new AttributesTable
            {
                { nameof(waypoint.TimestampUtc), waypoint.TimestampUtc },
                { nameof(waypoint.Name), waypoint.Name },
                { nameof(waypoint.Description), waypoint.Description },
                { nameof(waypoint.SymbolText), waypoint.SymbolText },
                { nameof(waypoint.MagneticVariation), waypoint.MagneticVariation },
                { nameof(waypoint.GeoidHeight), waypoint.GeoidHeight },
                { nameof(waypoint.Comment), waypoint.Comment },
                { nameof(waypoint.Source), waypoint.Source },
                { nameof(waypoint.Links), waypoint.Links },
                { nameof(waypoint.Classification), waypoint.Classification },
                { nameof(waypoint.FixKind), waypoint.FixKind },
                { nameof(waypoint.NumberOfSatellites), waypoint.NumberOfSatellites },
                { nameof(waypoint.HorizontalDilutionOfPrecision), waypoint.HorizontalDilutionOfPrecision },
                { nameof(waypoint.VerticalDilutionOfPrecision), waypoint.VerticalDilutionOfPrecision },
                { nameof(waypoint.PositionDilutionOfPrecision), waypoint.PositionDilutionOfPrecision },
                { nameof(waypoint.SecondsSinceLastDgpsUpdate), waypoint.SecondsSinceLastDgpsUpdate },
                { nameof(waypoint.DgpsStationId), waypoint.DgpsStationId },
                { nameof(waypoint.Extensions), waypoint.Extensions },
            };

            return new Feature(point, attributes);
        }

        /// <summary>
        /// Creates an <see cref="ILineString"/> <see cref="Feature"/> that contains the same data
        /// stored in a given <see cref="GpxRoute"/>.
        /// </summary>
        /// <param name="route">
        /// The <see cref="GpxRoute"/> source.
        /// </param>
        /// <param name="geometryFactory">
        /// The <see cref="IGeometryFactory"/> to use to create the <see cref="ILineString"/>, or
        /// <see langword="null"/> to create a new one to use on-the-fly using the current
        /// <see cref="GeometryServiceProvider"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Feature"/> that contains the same data as <paramref name="route"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="route"/> is <see langword="null"/>
        /// </exception>
        /// <remarks>
        /// The values of <see cref="Feature.Attributes"/> will be populated with the values of the
        /// <see cref="GpxRoute"/> properties, even when <see langword="null"/>.
        /// </remarks>
        public static Feature ToFeature(GpxRoute route, IGeometryFactory geometryFactory)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (geometryFactory is null)
            {
                geometryFactory = GeometryServiceProvider.Instance.CreateGeometryFactory();
            }

            // a route is an ILineString feature.
            var lineString = BuildLineString(route.Waypoints, geometryFactory);
            var attributes = new AttributesTable
            {
                { nameof(route.Name), route.Name },
                { nameof(route.Comment), route.Comment },
                { nameof(route.Description), route.Description },
                { nameof(route.Source), route.Source },
                { nameof(route.Links), route.Links },
                { nameof(route.Number), route.Number },
                { nameof(route.Classification), route.Classification },
                { nameof(route.Waypoints), route.Waypoints },
                { nameof(route.Extensions), route.Extensions },
            };

            return new Feature(lineString, attributes);
        }

        /// <summary>
        /// Creates an <see cref="IMultiLineString"/> <see cref="Feature"/> that contains the same data
        /// stored in a given <see cref="GpxTrack"/>.
        /// </summary>
        /// <param name="track">
        /// The <see cref="GpxTrack"/> source.
        /// </param>
        /// <param name="geometryFactory">
        /// The <see cref="IGeometryFactory"/> to use to create the <see cref="IMultiLineString"/>,
        /// or <see langword="null"/> to create a new one to use on-the-fly using the current
        /// <see cref="GeometryServiceProvider"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Feature"/> that contains the same data as <paramref name="track"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="track"/> is <see langword="null"/>
        /// </exception>
        /// <remarks>
        /// The values of <see cref="Feature.Attributes"/> will be populated with the values of the
        /// <see cref="GpxTrack"/> properties, even when <see langword="null"/>.
        /// </remarks>
        public static Feature ToFeature(GpxTrack track, IGeometryFactory geometryFactory)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (geometryFactory is null)
            {
                geometryFactory = GeometryServiceProvider.Instance.CreateGeometryFactory();
            }

            // a track is an IMultiLineString feature.
            var lineStrings = new ILineString[track.Segments.Length];
            for (int i = 0; i < lineStrings.Length; i++)
            {
                lineStrings[i] = BuildLineString(track.Segments[i].Waypoints, geometryFactory);
            }

            var multiLineString = geometryFactory.CreateMultiLineString(lineStrings);
            var attributes = new AttributesTable
            {
                { nameof(track.Name), track.Name },
                { nameof(track.Comment), track.Comment },
                { nameof(track.Description), track.Description },
                { nameof(track.Source), track.Source },
                { nameof(track.Links), track.Links },
                { nameof(track.Number), track.Number },
                { nameof(track.Classification), track.Classification },
                { nameof(track.Segments), track.Segments },
                { nameof(track.Extensions), track.Extensions },
            };

            return new Feature(multiLineString, attributes);
        }

        private static ILineString BuildLineString(ImmutableGpxWaypointTable waypoints, IGeometryFactory geometryFactory)
        {
            ICoordinateSequence coords;
            if (waypoints.Count == 1)
            {
                var waypoint = waypoints[0];
                coords = geometryFactory.CoordinateSequenceFactory.Create(waypoints.Count, 2);
                coords.SetOrdinate(0, Ordinate.X, waypoint.Longitude);
                coords.SetOrdinate(1, Ordinate.X, waypoint.Longitude);
                coords.SetOrdinate(0, Ordinate.Y, waypoint.Latitude);
                coords.SetOrdinate(1, Ordinate.Y, waypoint.Latitude);
                if (waypoint.ElevationInMeters.HasValue)
                {
                    coords.SetOrdinate(0, Ordinate.Z, waypoint.ElevationInMeters.Value);
                    coords.SetOrdinate(1, Ordinate.Z, waypoint.ElevationInMeters.Value);
                }
            }
            else
            {
                coords = geometryFactory.CoordinateSequenceFactory.Create(waypoints.Count, 3);
                for (int i = 0; i < waypoints.Count; i++)
                {
                    var waypoint = waypoints[i];
                    coords.SetOrdinate(i, Ordinate.X, waypoint.Longitude);
                    coords.SetOrdinate(i, Ordinate.Y, waypoint.Latitude);
                    if (waypoint.ElevationInMeters.HasValue)
                    {
                        coords.SetOrdinate(i, Ordinate.Z, waypoint.ElevationInMeters.GetValueOrDefault());
                    }
                }
            }

            return geometryFactory.CreateLineString(coords);
        }

        private static IEnumerable<GpxWaypoint> GenerateWaypoints(ILineString rte, Coordinate coord = null)
        {
            var seq = rte.CoordinateSequence;
            coord = coord ?? new Coordinate();
            for (int i = 0, cnt = seq.Count; i < cnt; i++)
            {
                seq.GetCoordinate(i, coord);
                yield return new GpxWaypoint(longitude: new GpxLongitude(coord.X),
                                             latitude: new GpxLatitude(coord.Y),
                                             elevationInMeters: double.IsNaN(coord.Z) ? default(double?) : coord.Z);
            }
        }

        private static IEnumerable<GpxTrackSegment> GenerateTrackSegments(IMultiLineString lineStrings)
        {
            var coord = new Coordinate();
            foreach (ILineString lineString in lineStrings)
            {
                yield return new GpxTrackSegment(waypoints: new ImmutableGpxWaypointTable(GenerateWaypoints(lineString, coord)),
                                                 extensions: null);
            }
        }
    }
}
