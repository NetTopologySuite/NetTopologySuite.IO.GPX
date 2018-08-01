using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents two lat/lon pairs defining the extent of an element.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_boundsType">boundsType</a>".
    /// </remarks>
    public sealed class GpxBoundingBox : ICanWriteToXmlWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxBoundingBox"/> class.
        /// </summary>
        /// <param name="minLongitude">
        /// The value for <see cref="MinLongitude"/>.
        /// </param>
        /// <param name="minLatitude">
        /// The value for <see cref="MinLatitude"/>.
        /// </param>
        /// <param name="maxLongitude">
        /// The value for <see cref="MaxLongitude"/>.
        /// </param>
        /// <param name="maxLatitude">
        /// The value for <see cref="MaxLatitude"/>.
        /// </param>
        public GpxBoundingBox(GpxLongitude minLongitude, GpxLatitude minLatitude, GpxLongitude maxLongitude, GpxLatitude maxLatitude)
        {
            this.MinLongitude = minLongitude;
            this.MinLatitude = minLatitude;
            this.MaxLongitude = maxLongitude;
            this.MaxLatitude = maxLatitude;
        }

        /// <summary>
        /// Gets the minimum <see cref="GpxLongitude"/> value.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "minlon" attribute.
        /// </remarks>
        public GpxLongitude MinLongitude { get; }

        /// <summary>
        /// Gets the minimum <see cref="GpxLatitude"/> value.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "minlat" attribute.
        /// </remarks>
        public GpxLatitude MinLatitude { get; }

        /// <summary>
        /// Gets the maximum <see cref="GpxLongitude"/> value.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "maxlon" attribute.
        /// </remarks>
        public GpxLongitude MaxLongitude { get; }

        /// <summary>
        /// Gets the maximum <see cref="GpxLatitude"/> value.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "maxlat" attribute.
        /// </remarks>
        public GpxLatitude MaxLatitude { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.MinLongitude), this.MinLongitude),
                                                                 (nameof(this.MinLatitude), this.MinLatitude),
                                                                 (nameof(this.MaxLongitude), this.MaxLongitude),
                                                                 (nameof(this.MaxLatitude), this.MaxLatitude));

        internal static GpxBoundingBox Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxBoundingBox(
                minLongitude: Helpers.ParseLongitude(element.Attribute("minlon")?.Value) ?? throw new XmlException("bounds element must have minlon attribute"),
                minLatitude: Helpers.ParseLatitude(element.Attribute("minlat")?.Value) ?? throw new XmlException("bounds element must have minlat attribute"),
                maxLongitude: Helpers.ParseLongitude(element.Attribute("maxlon")?.Value) ?? throw new XmlException("bounds element must have maxlon attribute"),
                maxLatitude: Helpers.ParseLatitude(element.Attribute("maxlat")?.Value) ?? throw new XmlException("bounds element must have maxlat attribute"));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("minlat", this.MinLatitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("minlon", this.MinLongitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxlat", this.MaxLatitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxlon", this.MaxLongitude.Value.ToRoundTripString(CultureInfo.InvariantCulture));
        }
    }
}
