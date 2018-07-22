using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public static class GpxWriter
    {
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
