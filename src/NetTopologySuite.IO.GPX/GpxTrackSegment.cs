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
        /// <param name="waypoints">
        /// The value for <see cref="Waypoints"/>.
        /// </param>
        /// <param name="extensions">
        /// The value for <see cref="Extensions"/>.
        /// </param>
        public GpxTrackSegment(ImmutableGpxWaypointTable waypoints, object extensions)
        {
            this.Waypoints = waypoints;
            this.Extensions = extensions;
        }

        /// <summary>
        /// Gets the waypoints that make up this segment.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "trkpt" elements.
        /// </remarks>
        public ImmutableGpxWaypointTable Waypoints { get; }

        /// <summary>
        /// Gets arbitrary extension information associated with this segment.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions { get; }

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

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Waypoints), Helpers.BuildString((nameof(this.Waypoints.Count), this.Waypoints.Count))),
                                                                 (nameof(this.Extensions), this.Extensions));

        internal void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            Func<object, IEnumerable<XElement>> extensionCallback = settings.ExtensionWriter.ConvertTrackPointExtension;
            foreach (var waypoint in this.Waypoints)
            {
                writer.WriteGpxStartElement("trkpt");
                waypoint.Save(writer, settings, extensionCallback);
                writer.WriteEndElement();
            }

            writer.WriteExtensions(this.Extensions, settings.ExtensionWriter.ConvertTrackSegmentExtension);
        }
    }
}
