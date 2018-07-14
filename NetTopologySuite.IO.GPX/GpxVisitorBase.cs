using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public abstract class GpxVisitorBase
    {
        public virtual void VisitMetadata(GpxMetadata metadata) { }

        public virtual void VisitWaypoint(GpxWaypoint waypoint) { }

        public virtual void VisitRoute(GpxRoute route) { }

        public virtual void VisitTrack(GpxTrack track) { }

        public virtual object ConvertMetadataExtensionElement(XElement extensionElement) => null;

        public virtual object ConvertWaypointExtensionElement(XElement extensionElement) => null;

        public virtual object ConvertRouteExtensionElement(XElement extensionElement) => null;

        public virtual object ConvertTrackExtensionElement(XElement extensionElement) => null;

        public virtual object ConvertTrackSegmentExtensionElement(XElement extensionElement) => null;
    }
}
