using System;
using System.Collections.Generic;

using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public class NetTopologySuiteFeatureBuilderVisitor : GpxVisitorBase
    {
        private readonly IGeometryFactory geometryFactory;

        private readonly List<Feature> currentFeatures = new List<Feature>();

        private GpxMetadata currentMetadata;

        private object currentExtensions;

        public NetTopologySuiteFeatureBuilderVisitor(IGeometryFactory geometryFactory)
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
            var attributes = new AttributesTable { { "wpt", waypoint } };
            var feature = new Feature(point, attributes);
            this.currentFeatures.Add(feature);
        }

        public sealed override void VisitRoute(GpxRoute route)
        {
            base.VisitRoute(route);

            // a route is an ILineString feature.
            var lineString = this.BuildLineString(route.Waypoints);
            var attributes = new AttributesTable { { "rte", route } };
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
            var attributes = new AttributesTable { { "trk", track } };
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
