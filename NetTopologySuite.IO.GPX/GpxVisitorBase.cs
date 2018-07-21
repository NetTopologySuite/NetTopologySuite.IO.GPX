namespace NetTopologySuite.IO
{
    public abstract class GpxVisitorBase
    {
        public virtual void VisitMetadata(GpxMetadata metadata) { }

        public virtual void VisitWaypoint(GpxWaypoint waypoint) { }

        public virtual void VisitRoute(GpxRoute route) { }

        public virtual void VisitTrack(GpxTrack track) { }

        public virtual void VisitExtensions(object extensions) { }
    }
}
