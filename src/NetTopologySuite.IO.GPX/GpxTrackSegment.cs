using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a continuous span of track data, logically connected in order.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_trksegType">trksegType</a>".
    /// </remarks>
    public sealed class GpxTrackSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxTrackSegment"/> class.
        /// </summary>
        public GpxTrackSegment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxTrackSegment"/> class.
        /// </summary>
        /// <param name="waypoints">
        /// The value for <see cref="Waypoints"/>.
        /// </param>
        /// <param name="extensions">
        /// The value for <see cref="Extensions"/>.
        /// </param>
        public GpxTrackSegment(ImmutableGpxWaypointTable waypoints, object extensions)
        {
            if (waypoints != null)
            {
                Waypoints = waypoints;
            }

            Extensions = extensions;
        }

        /// <summary>
        /// Gets the waypoints that make up this segment.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "trkpt" elements.
        /// </remarks>
        public ImmutableGpxWaypointTable Waypoints { get; } = ImmutableGpxWaypointTable.Empty;

        /// <summary>
        /// Gets arbitrary extension information associated with this segment.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions { get; }

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrackSegment"/> as a copy of this instance, but with
        /// <see cref="Waypoints"/> replaced by the given value.
        /// </summary>
        /// <param name="waypoints">
        /// The new value for <see cref="Waypoints"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrackSegment"/> instance that's a copy of the current instance, but
        /// with its <see cref="Waypoints"/> value set to <paramref name="waypoints"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="waypoints"/> contains <see langword="null"/> elements.
        /// </exception>
        public GpxTrackSegment WithWaypoints(IEnumerable<GpxWaypoint> waypoints) => new GpxTrackSegment(new ImmutableGpxWaypointTable(waypoints ?? ImmutableGpxWaypointTable.Empty), Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrackSegment"/> as a copy of this instance, but with
        /// <see cref="Extensions"/> replaced by the given value.
        /// </summary>
        /// <param name="extensions">
        /// The new value for <see cref="Extensions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrackSegment"/> instance that's a copy of the current instance, but
        /// with its <see cref="Extensions"/> value set to <paramref name="extensions"/>.
        /// </returns>
        public GpxTrackSegment WithExtensions(object extensions) => new GpxTrackSegment(Waypoints, extensions);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxTrackSegment other &&
                                                   Equals(Waypoints, other.Waypoints) &&
                                                   Equals(Extensions, other.Extensions);

        /// <inheritdoc />
        public override int GetHashCode() => (Waypoints, Extensions).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(Waypoints), Helpers.BuildString((nameof(Waypoints.Count), Waypoints.Count))),
                                                                 (nameof(Extensions), Extensions));

        internal static GpxTrackSegment Load(XElement element, GpxReaderSettings settings)
        {
            if (element is null)
            {
                return null;
            }

            var extensionsElement = element.GpxElement("extensions");
            return new GpxTrackSegment(
                waypoints: new ImmutableGpxWaypointTable(element.GpxElements("trkpt"), settings, settings.ExtensionReader.ConvertTrackPointExtensionElement),
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertTrackSegmentExtensionElement(extensionsElement.Elements()));
        }

        internal void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            Func<object, IEnumerable<XElement>> extensionCallback = settings.ExtensionWriter.ConvertTrackPointExtension;
            foreach (var waypoint in Waypoints)
            {
                writer.WriteGpxStartElement("trkpt");
                waypoint.Save(writer, settings, extensionCallback);
                writer.WriteEndElement();
            }

            writer.WriteExtensions(Extensions, settings.ExtensionWriter.ConvertTrackSegmentExtension);
        }
    }
}
