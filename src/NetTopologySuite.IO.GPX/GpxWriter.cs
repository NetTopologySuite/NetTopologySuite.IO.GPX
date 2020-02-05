using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using NetTopologySuite.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Provides methods that write out GPX data to a <see cref="XmlWriter"/>.
    /// </summary>
    public static class GpxWriter
    {
        /// <summary>
        /// Writes the given features to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write to.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxWriterSettings"/> instance to use to control how GPX instances get
        /// written out, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <param name="metadata">
        /// The <see cref="GpxMetadata"/> instance that includes metadata about the file.  Required.
        /// </param>
        /// <param name="waypointFeatures">
        /// The <see cref="IFeature"/>s that include the top-level wpt instances.  Optional, but if
        /// specified, each must be a <see cref="Point"/> feature.
        /// </param>
        /// <param name="routeFeatures">
        /// The <see cref="IFeature"/>s that include the top-level rte instances.  Optional, but if
        /// specified, each must be a <see cref="LineString"/> feature.
        /// </param>
        /// <param name="trackFeatures">
        /// The <see cref="IFeature"/>s that include the top-level trk instances.  Optional, but if
        /// specified, each must be a <see cref="MultiLineString"/> feature.
        /// </param>
        /// <param name="extensions">
        /// The top-level extension data.  Optional.
        /// </param>
        public static void Write(XmlWriter writer, GpxWriterSettings settings, GpxMetadata metadata, IEnumerable<IFeature> waypointFeatures, IEnumerable<IFeature> routeFeatures, IEnumerable<IFeature> trackFeatures, object extensions) =>
            Write(writer,
                  settings,
                  metadata,
                  waypointFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxWaypoint),
                  routeFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxRoute),
                  trackFeatures?.Select(NetTopologySuiteGpxFeatureConverter.ToGpxTrack),
                  extensions);

        /// <summary>
        /// Writes the given features to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write to.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxWriterSettings"/> instance to use to control how GPX instances get
        /// written out, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <param name="metadata">
        /// The <see cref="GpxMetadata"/> instance that includes metadata about the file.  Required.
        /// </param>
        /// <param name="features">
        /// The <see cref="IFeature"/>s to write out.  Required.
        /// </param>
        /// <param name="extensions">
        /// The top-level extension data.  Optional.
        /// </param>
        /// <remarks>
        /// The top-level GPX data objects written out depend on the <see cref="IFeature.Geometry"/>
        /// type, as follows:
        /// <list type="table">
        /// <listheader>
        /// <term>Type of <see cref="IFeature.Geometry"/></term>
        /// <description>Corresponding top-level data object</description>
        /// </listheader>
        /// <item>
        /// <term><see cref="Point"/></term>
        /// <description><see cref="GpxWaypoint"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="LineString"/></term>
        /// <description><see cref="GpxRoute"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="MultiLineString"/></term>
        /// <description><see cref="GpxTrack"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="writer"/>, <paramref name="metadata"/>, or
        /// <paramref name="features"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when an element of <paramref name="features"/> or its
        /// <see cref="IFeature.Geometry"/> is <see langword="null" />, or is not an instance of one
        /// of the recognized geometry types (see remarks).
        /// </exception>
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

                    case Point _:
                        waypoints.Add(NetTopologySuiteGpxFeatureConverter.ToGpxWaypoint(feature));
                        break;

                    case LineString _:
                        routes.Add(NetTopologySuiteGpxFeatureConverter.ToGpxRoute(feature));
                        break;

                    case MultiLineString _:
                        tracks.Add(NetTopologySuiteGpxFeatureConverter.ToGpxTrack(feature));
                        break;

                    default:
                        throw new ArgumentException("All features must be either Point (for wpt), LineString (for rte), or MultiLineString (for trk).  Not " + feature.Geometry.GetType(), nameof(features));
                }
            }

            Write(writer, settings, metadata, waypoints, routes, tracks, extensions);
        }

        /// <summary>
        /// Writes the given features to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write to.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxWriterSettings"/> instance to use to control how GPX instances get
        /// written out, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <param name="metadata">
        /// The <see cref="GpxMetadata"/> instance that includes metadata about the file.  Required.
        /// </param>
        /// <param name="waypoints">
        /// The top-level wpt instances to write out.  Optional, but if specified, each element must
        /// be non-<see langword="null"/>.
        /// </param>
        /// <param name="routes">
        /// The top-level rte instances to write out.  Optional, but if specified, each element must
        /// be non-<see langword="null"/>.
        /// </param>
        /// <param name="tracks">
        /// The top-level trk instances to write out.  Optional, but if specified, each element must
        /// be non-<see langword="null"/>.
        /// </param>
        /// <param name="extensions">
        /// The top-level extension data.  Optional.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="writer"/> or <paramref name="metadata"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when an element of <paramref name="waypoints"/>, <paramref name="routes"/>, or
        /// <paramref name="tracks"/> is <see langword="null"/>.
        /// </exception>
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

            foreach (var (desiredPrefix, namespaceUri) in settings.CommonXmlNamespacesByDesiredPrefix)
            {
                writer.WriteAttributeString("xmlns", desiredPrefix, null, namespaceUri.ToString());
            }

            if (!metadata.IsTrivial)
            {
                writer.WriteGpxStartElement("metadata");
                metadata.Save(writer, settings);
                writer.WriteEndElement();
            }

            Func<object, IEnumerable<XElement>> waypointExtensionCallback = settings.ExtensionWriter.ConvertWaypointExtension;
            foreach (var waypoint in waypoints ?? Enumerable.Empty<GpxWaypoint>())
            {
                if (waypoint is null)
                {
                    throw new ArgumentException("All waypoints must be non-null.", nameof(waypoints));
                }

                writer.WriteGpxStartElement("wpt");
                waypoint.Save(writer, settings, waypointExtensionCallback);
                writer.WriteEndElement();
            }

            foreach (var route in routes ?? Enumerable.Empty<GpxRoute>())
            {
                if (route is null)
                {
                    throw new ArgumentException("All routes must be non-null.", nameof(routes));
                }

                writer.WriteGpxStartElement("rte");
                route.Save(writer, settings);
                writer.WriteEndElement();
            }

            foreach (var track in tracks ?? Enumerable.Empty<GpxTrack>())
            {
                if (track is null)
                {
                    throw new ArgumentException("All tracks must be non-null.", nameof(tracks));
                }

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
