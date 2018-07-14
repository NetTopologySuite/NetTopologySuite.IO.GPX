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

        public static GpxTrackSegment Load(XElement element, GpxReaderSettings settings, Func<XElement, object> trackSegmentExtensionCallback, Func<XElement, object> waypointExtensionCallback)
        {
            return LoadNoValidation(element,
                                    settings ?? throw new ArgumentNullException(nameof(settings)),
                                    trackSegmentExtensionCallback ?? throw new ArgumentNullException(nameof(trackSegmentExtensionCallback)),
                                    waypointExtensionCallback ?? throw new ArgumentNullException(nameof(waypointExtensionCallback)));
        }

        internal static GpxTrackSegment LoadNoValidation(XElement element, GpxReaderSettings settings, Func<XElement, object> trackSegmentExtensionCallback, Func<XElement, object> waypointExtensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxTrackSegment(
                waypoints: new ImmutableGpxWaypointTable(element.GpxElements("trkpt"), settings, waypointExtensionCallback),
                extensions: trackSegmentExtensionCallback(element.GpxElement("extensions")));
        }
    }
}
