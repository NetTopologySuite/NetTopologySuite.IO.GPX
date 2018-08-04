using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public static class GpxWriter
    {
        public static void Write(XmlWriter writer, GpxWriterSettings settings, GpxMetadata metadata, IEnumerable<IFeature> waypointFeatures, IEnumerable<IFeature> routeFeatures, IEnumerable<IFeature> trackFeatures, object extensions)
        {
            Write(writer,
                  settings,
                  metadata,
                  waypointFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxWaypoint),
                  routeFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxRoute),
                  trackFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxTrack),
                  extensions);
        }

        public static void Write(XmlWriter writer, GpxWriterSettings settings, GpxMetadata metadata, IEnumerable<IFeature> features, object extensions)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (features is null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            var waypoints = new List<GpxWaypoint>();
            var routes = new List<GpxRoute>();
            var tracks = new List<GpxTrack>();
            foreach (var feature in features)
            {
                switch (feature?.Geometry)
                {
                    case null:
                        throw new ArgumentException("All features must be non-null and contain non-null geometries.", nameof(features));

                    case IPoint _:
                        waypoints.Add(NetTopologySuiteGpxFeatureConverter.ToGpxWaypoint(feature));
                        break;

                    case ILineString _:
                        routes.Add(NetTopologySuiteGpxFeatureConverter.ToGpxRoute(feature));
                        break;

                    case IMultiLineString _:
                        tracks.Add(NetTopologySuiteGpxFeatureConverter.ToGpxTrack(feature));
                        break;

                    default:
                        throw new ArgumentException("All features must be either IPoint (for wpt), ILineString (for rte), or IMultiLineString (for trk).  Not " + feature.Geometry.GetType(), nameof(features));
                }
            }

            Write(writer, settings, metadata, waypoints, routes, tracks, extensions);
        }

        public static void Write(XmlWriter writer, GpxWriterSettings settings, GpxMetadata metadata, IEnumerable<GpxWaypoint> waypoints, IEnumerable<GpxRoute> routes, IEnumerable<GpxTrack> tracks, object extensions)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            settings = settings ?? new GpxWriterSettings();

            writer.WriteStartDocument();
            writer.WriteGpxStartElement("gpx");
            writer.WriteAttributeString("version", "1.1");
            writer.WriteAttributeString("creator", metadata.Creator);
            if (!metadata.IsTrivial)
            {
                writer.WriteGpxStartElement("metadata");
                metadata.Save(writer, settings);
                writer.WriteEndElement();
            }

            Func<object, IEnumerable<XElement>> waypointExtensionCallback = settings.ExtensionWriter.ConvertWaypointExtension;
            foreach (var waypoint in waypoints ?? Enumerable.Empty<GpxWaypoint>())
            {
                writer.WriteGpxStartElement("wpt");
                waypoint.Save(writer, settings, waypointExtensionCallback);
                writer.WriteEndElement();
            }

            foreach (var route in routes ?? Enumerable.Empty<GpxRoute>())
            {
                writer.WriteGpxStartElement("rte");
                route.Save(writer, settings);
                writer.WriteEndElement();
            }

            foreach (var track in tracks ?? Enumerable.Empty<GpxTrack>())
            {
                writer.WriteGpxStartElement("trk");
                track.Save(writer, settings);
                writer.WriteEndElement();
            }

            writer.WriteExtensions(extensions, settings.ExtensionWriter.ConvertGpxExtension);

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }
}
