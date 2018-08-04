using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public class GpxExtensionWriter
    {
        public virtual IEnumerable<XElement> ConvertGpxExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertMetadataExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertWaypointExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertRouteExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertRoutePointExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertTrackExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertTrackSegmentExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();

        public virtual IEnumerable<XElement> ConvertTrackPointExtension(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();
    }
}
