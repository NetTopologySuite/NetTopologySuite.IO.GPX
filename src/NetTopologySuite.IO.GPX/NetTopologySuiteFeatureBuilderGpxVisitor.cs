using System;
using System.Collections.Generic;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO
{
    internal sealed class NetTopologySuiteFeatureBuilderGpxVisitor : GpxVisitorBase
    {
        private readonly GeometryFactory _geometryFactory;

        private readonly List<Feature> _currentFeatures = new List<Feature>();

        private GpxMetadata _currentMetadata;

        private object _currentExtensions;

        public NetTopologySuiteFeatureBuilderGpxVisitor(GeometryFactory geometryFactory) => _geometryFactory = geometryFactory;

        public override void VisitMetadata(GpxMetadata metadata)
        {
            base.VisitMetadata(metadata);
            _currentMetadata = metadata;
        }

        public sealed override void VisitWaypoint(GpxWaypoint waypoint)
        {
            base.VisitWaypoint(waypoint);
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(waypoint, _geometryFactory);
            _currentFeatures.Add(feature);
        }

        public sealed override void VisitRoute(GpxRoute route)
        {
            base.VisitRoute(route);
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(route, _geometryFactory);
            _currentFeatures.Add(feature);
        }

        public sealed override void VisitTrack(GpxTrack track)
        {
            base.VisitTrack(track);
            var feature = NetTopologySuiteGpxFeatureConverter.ToFeature(track, _geometryFactory);
            _currentFeatures.Add(feature);
        }

        public override void VisitExtensions(object extensions)
        {
            base.VisitExtensions(extensions);
            _currentExtensions = extensions;
        }

        public (GpxMetadata metadata, Feature[] features, object extensions) Terminate()
        {
            var result = (_currentMetadata, _currentFeatures.ToArray(), _currentExtensions);
            _currentMetadata = null;
            _currentFeatures.Clear();
            _currentExtensions = null;
            return result;
        }
    }
}
