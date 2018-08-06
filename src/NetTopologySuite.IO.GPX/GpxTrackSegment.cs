using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxTrackSegment
    {
        public GpxTrackSegment(ImmutableGpxWaypointTable waypoints, object extensions)
        {
            this.Waypoints = waypoints;
            this.Extensions = extensions;
        }

        public ImmutableGpxWaypointTable Waypoints { get; }

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
