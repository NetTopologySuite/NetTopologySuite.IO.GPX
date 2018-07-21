using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxBoundingBox : ICanWriteToXmlWriter
    {
        public GpxBoundingBox(GpxLongitude minLongitude, GpxLatitude minLatitude, GpxLongitude maxLongitude, GpxLatitude maxLatitude)
        {
            this.MinLongitude = minLongitude;
            this.MinLatitude = minLatitude;
            this.MaxLongitude = maxLongitude;
            this.MaxLatitude = maxLatitude;
        }

        public static GpxBoundingBox Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxBoundingBox(
                minLongitude: Helpers.ParseLongitude(element.GpxAttribute("minlon")?.Value) ?? throw new XmlException("bounds element must have minlon attribute"),
                minLatitude: Helpers.ParseLatitude(element.GpxAttribute("minlat")?.Value) ?? throw new XmlException("bounds element must have minlat attribute"),
                maxLongitude: Helpers.ParseLongitude(element.GpxAttribute("maxlon")?.Value) ?? throw new XmlException("bounds element must have maxlon attribute"),
                maxLatitude: Helpers.ParseLatitude(element.GpxAttribute("maxlat")?.Value) ?? throw new XmlException("bounds element must have maxlat attribute"));
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteAttributeString("minlat", this.MinLatitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("minlon", this.MinLongitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxlat", this.MaxLatitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxlon", this.MaxLongitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
        }

        public GpxLongitude MinLongitude { get; }

        public GpxLatitude MinLatitude { get; }

        public GpxLongitude MaxLongitude { get; }

        public GpxLatitude MaxLatitude { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.MinLongitude), this.MinLongitude),
                                                                 (nameof(this.MinLatitude), this.MinLatitude),
                                                                 (nameof(this.MaxLongitude), this.MaxLongitude),
                                                                 (nameof(this.MaxLatitude), this.MaxLatitude));
    }
}
