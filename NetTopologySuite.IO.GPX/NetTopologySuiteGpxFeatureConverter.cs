using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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
