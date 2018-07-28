using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GeoAPI;
using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public static class NetTopologySuiteGpxFeatureConverter
    {
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
            return new GpxWaypoint(longitude: new GpxLongitude(wpt.X),
                                   latitude: new GpxLatitude(wpt.Y),
                                   elevationInMeters: double.IsNaN(wpt.Z) ? default(double?) : wpt.Z,
                                   timestampUtc: (DateTime?)attributes?.GetOptionalValue(nameof(GpxWaypoint.TimestampUtc)),
                                   name: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Name)),
                                   description: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Description)),
                                   symbolText: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.SymbolText)),
                                   magneticVariation: (GpxDegrees?)attributes?.GetOptionalValue(nameof(GpxWaypoint.MagneticVariation)),
                                   geoidHeight: (double?)attributes?.GetOptionalValue(nameof(GpxWaypoint.GeoidHeight)),
                                   comment: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Comment)),
                                   source: (string)attributes?.GetOptionalValue(nameof(GpxWaypoint.Source)),
                                   links: attributes?.GetOptionalValue(nameof(GpxWaypoint.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
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
            return new GpxRoute(name: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Name)),
                                comment: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Comment)),
                                description: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Description)),
                                source: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Source)),
                                links: attributes?.GetOptionalValue(nameof(GpxRoute.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
                                number: (uint?)attributes?.GetOptionalValue(nameof(GpxRoute.Number)),
                                classification: (string)attributes?.GetOptionalValue(nameof(GpxRoute.Classification)),
                                waypoints: new ImmutableGpxWaypointTable(((IEnumerable<GpxWaypoint>)attributes?.GetOptionalValue(nameof(GpxRoute.Waypoints))) ?? GenerateWaypoints(rte)),
                                extensions: attributes?.GetOptionalValue(nameof(GpxRoute.Extensions)));
        }

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

            return new GpxTrack(name: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Name)),
                                comment: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Comment)),
                                description: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Description)),
                                source: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Source)),
                                links: attributes?.GetOptionalValue(nameof(GpxTrack.Links)) is ImmutableArray<GpxWebLink> storedLinks && !storedLinks.IsDefaultOrEmpty ? storedLinks : ImmutableArray<GpxWebLink>.Empty,
                                number: (uint?)attributes?.GetOptionalValue(nameof(GpxTrack.Number)),
                                classification: (string)attributes?.GetOptionalValue(nameof(GpxTrack.Classification)),
                                segments: ImmutableArray.CreateRange(trackSegments ?? GenerateTrackSegments(trk)),
                                extensions: attributes?.GetOptionalValue(nameof(GpxTrack.Extensions)));
        }

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
            Coordinate[] coords;
            if (waypoints.Count == 1)
            {
                var waypoint = waypoints[0];
                coords = new Coordinate[2];
                coords[0] = coords[1] = new Coordinate(waypoint.Longitude, waypoint.Latitude, waypoint.ElevationInMeters ?? Coordinate.NullOrdinate);
            }
            else
            {
                coords = new Coordinate[waypoints.Count];
                for (int i = 0; i < coords.Length; i++)
                {
                    var waypoint = waypoints[i];
                    coords[i] = new Coordinate(waypoint.Longitude, waypoint.Latitude, waypoint.ElevationInMeters ?? Coordinate.NullOrdinate);
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
                                             elevationInMeters: double.IsNaN(coord.Z) ? default(double?) : coord.Z,
                                             timestampUtc: null,
                                             name: null,
                                             description: null,
                                             symbolText: null);
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
