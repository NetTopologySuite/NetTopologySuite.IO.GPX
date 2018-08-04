using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents an e-mail address by ID and domain, kept separate in GPX files in an attempt to
    /// defeat simple e-mail harvesting schemes.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_emailType">emailType</a>".
    /// </remarks>
    public sealed class GpxEmail : ICanWriteToXmlWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxEmail"/> class.
        /// </summary>
        /// <param name="id">
        /// The value for <see cref="Id"/>.
        /// </param>
        /// <param name="domain">
        /// The value for <see cref="Domain"/>.
        /// </param>
        public GpxEmail(string id, string domain)
        {
            this.Id = id;
            this.Domain = domain;
        }

        /// <summary>
        /// Gets the user ID fragment of the e-mail address,
        /// This is the part before the '@'.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "id" attribute.
        /// </remarks>
        public string Id { get; }

        /// <summary>
        /// Gets the user ID fragment of the e-mail address,
        /// This is the part after the '@'.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "domain" attribute.
        /// </remarks>
        public string Domain { get; }

        internal static GpxEmail Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxEmail(
                id: element.Attribute("id")?.Value ?? throw new XmlException("email element must have both 'id' and 'domain' attributes"),
                domain: element.Attribute("domain")?.Value ?? throw new XmlException("email element must have both 'id' and 'domain' attributes"));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("id", this.Id);
            writer.WriteAttributeString("domain", this.Domain);
        }

        /// <inheritdoc />
        public override string ToString() => this.Id + "@" + this.Domain;
    }
}
