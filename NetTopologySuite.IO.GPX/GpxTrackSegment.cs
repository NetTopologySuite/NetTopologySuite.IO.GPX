using System;
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

        public static GpxTrackSegment Load(XElement element, GpxReaderSettings settings)
        {
            return LoadNoValidation(element,
                                    settings ?? throw new ArgumentNullException(nameof(settings)));
        }

        internal static GpxTrackSegment LoadNoValidation(XElement element, GpxReaderSettings settings)
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
    }
}
