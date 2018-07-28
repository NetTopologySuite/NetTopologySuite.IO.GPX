using System;
using System.Collections.Generic;

using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public class NetTopologySuiteFeatureBuilderGpxVisitor : GpxVisitorBase
    {
        private readonly IGeometryFactory geometryFactory;

        private readonly List<Feature> currentFeatures = new List<Feature>();

        private GpxMetadata currentMetadata;

        private object currentExtensions;

        public NetTopologySuiteFeatureBuilderGpxVisitor(IGeometryFactory geometryFactory)
        {
            this.geometryFactory = geometryFactory ?? throw new ArgumentNullException(nameof(geometryFactory));
        }

        public override void VisitMetadata(GpxMetadata metadata)
        {
            base.VisitMetadata(metadata);
            this.currentMetadata = metadata;
        }

        public sealed override void VisitWaypoint(GpxWaypoint waypoint)
        {
            base.VisitWaypoint(waypoint);

            // a waypoint all on its own is an IPoint feature.
            var coord = new Coordinate(waypoint.Longitude, waypoint.Latitude, waypoint.ElevationInMeters ?? Coordinate.NullOrdinate);
            var point = this.geometryFactory.CreatePoint(coord);
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
            var feature = new Feature(point, attributes);
            this.currentFeatures.Add(feature);
        }

        public sealed override void VisitRoute(GpxRoute route)
        {
            base.VisitRoute(route);

            // a route is an ILineString feature.
            var lineString = this.BuildLineString(route.Waypoints);
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
            var feature = new Feature(lineString, attributes);
            this.currentFeatures.Add(feature);
        }

        public sealed override void VisitTrack(GpxTrack track)
        {
            base.VisitTrack(track);

            // a track is an IMultiLineString feature.
            var lineStrings = new ILineString[track.Segments.Length];
            for (int i = 0; i < lineStrings.Length; i++)
            {
                lineStrings[i] = this.BuildLineString(track.Segments[i].Waypoints);
            }

            var multiLineString = this.geometryFactory.CreateMultiLineString(lineStrings);
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
            var feature = new Feature(multiLineString, attributes);
            this.currentFeatures.Add(feature);
        }

        public override void VisitExtensions(object extensions)
        {
            base.VisitExtensions(extensions);
            this.currentExtensions = extensions;
        }

        public (GpxMetadata metadata, Feature[] features, object extensions) Terminate()
        {
            var result = (this.currentMetadata, this.currentFeatures.ToArray(), this.currentExtensions);
            this.currentMetadata = null;
            this.currentFeatures.Clear();
            this.currentExtensions = null;
            return result;
        }

        private ILineString BuildLineString(ImmutableGpxWaypointTable waypoints)
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

            return this.geometryFactory.CreateLineString(coords);
        }
    }
}
