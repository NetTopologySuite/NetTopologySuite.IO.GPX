using System;
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> or <paramref name="domain"/> is
        /// <see langword="null"/>.
        /// </exception>
        public GpxEmail(string id, string domain)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Domain = domain ?? throw new ArgumentNullException(nameof(domain));
        }

        /// <summary>
        /// Gets the user ID fragment of the e-mail address.
        /// This is the part before the '@'.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "id" attribute.
        /// </remarks>
        public string Id { get; }

        /// <summary>
        /// Gets the user ID fragment of the e-mail address.
        /// This is the part after the '@'.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "domain" attribute.
        /// </remarks>
        public string Domain { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxEmail other &&
                                                   Id == other.Id &&
                                                   Domain == other.Domain;

        /// <inheritdoc />
        public override int GetHashCode() => (Id, Domain).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Id + "@" + Domain;

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
            writer.WriteAttributeString("id", Id);
            writer.WriteAttributeString("domain", Domain);
        }
    }
}
