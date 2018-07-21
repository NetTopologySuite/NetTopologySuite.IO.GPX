using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public class GpxExtensionReader
    {
        public virtual object ConvertGpxExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertMetadataExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertWaypointExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertRouteExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertRoutePointExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertTrackExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertTrackSegmentExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();

        public virtual object ConvertTrackPointExtensionElement(IEnumerable<XElement> extensionElements) => extensionElements.ToArray();
    }
}
