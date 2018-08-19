using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a person or organization.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_personType">personType</a>".
    /// </remarks>
    public sealed class GpxPerson : ICanWriteToXmlWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxPerson"/> class.
        /// </summary>
        /// <param name="name">
        /// The value for <see cref="Name"/>.
        /// </param>
        /// <param name="email">
        /// The value for <see cref="Email"/>.
        /// </param>
        /// <param name="link">
        /// The value for <see cref="Link"/>.
        /// </param>
        public GpxPerson(string name, GpxEmail email, GpxWebLink link)
        {
            this.Name = name;
            this.Email = email;
            this.Link = link;
        }

        /// <summary>
        /// Gets the name of this person or organization.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "name" element.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the e-mail of this person or organization.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "email" element.
        /// </remarks>
        public GpxEmail Email { get; }

        /// <summary>
        /// Gets a link to external information about this person or organization.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" element.
        /// </remarks>
        public GpxWebLink Link { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Name), this.Name),
                                                                 (nameof(this.Email), this.Email),
                                                                 (nameof(this.Link), this.Link));

        internal static GpxPerson Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxPerson(
                name: element.GpxElement("name")?.Value,
                email: GpxEmail.Load(element.GpxElement("email")),
                link: GpxWebLink.Load(element.GpxElement("link")));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteOptionalGpxElementValue("name", this.Name);
            writer.WriteOptionalGpxElementValue("email", this.Email);
            writer.WriteOptionalGpxElementValue("link", this.Link);
        }
    }
}
