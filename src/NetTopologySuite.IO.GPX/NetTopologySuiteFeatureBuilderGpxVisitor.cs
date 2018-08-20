using System;
using System.Collections.Generic;

using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    internal sealed class NetTopologySuiteFeatureBuilderGpxVisitor : GpxVisitorBase
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
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(waypoint, this.geometryFactory);
            this.currentFeatures.Add(feature);
        }

        public sealed override void VisitRoute(GpxRoute route)
        {
            base.VisitRoute(route);
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(route, this.geometryFactory);
            this.currentFeatures.Add(feature);
        }

        public sealed override void VisitTrack(GpxTrack track)
        {
            base.VisitTrack(track);
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(track, this.geometryFactory);
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
    }
}
