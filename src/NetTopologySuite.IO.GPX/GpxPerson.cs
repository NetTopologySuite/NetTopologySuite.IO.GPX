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
        public GpxPerson()
        {
        }

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
            Name = name;
            Email = email;
            Link = link;
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

        /// <summary>
        /// Builds a new instance of <see cref="GpxPerson"/> as a copy of this instance, but with
        /// <see cref="Name"/> replaced by the given value.
        /// </summary>
        /// <param name="name">
        /// The new value for <see cref="Name"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxPerson"/> instance that's a copy of the current instance, but
        /// with its <see cref="Name"/> value set to <paramref name="name"/>.
        /// </returns>
        public GpxPerson WithName(string name) => new GpxPerson(name, Email, Link);

        /// <summary>
        /// Builds a new instance of <see cref="GpxPerson"/> as a copy of this instance, but with
        /// <see cref="Email"/> replaced by the given value.
        /// </summary>
        /// <param name="email">
        /// The new value for <see cref="Email"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxPerson"/> instance that's a copy of the current instance, but
        /// with its <see cref="Email"/> value set to <paramref name="email"/>.
        /// </returns>
        public GpxPerson WithEmail(GpxEmail email) => new GpxPerson(Name, email, Link);

        /// <summary>
        /// Builds a new instance of <see cref="GpxPerson"/> as a copy of this instance, but with
        /// <see cref="Link"/> replaced by the given value.
        /// </summary>
        /// <param name="link">
        /// The new value for <see cref="Link"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxPerson"/> instance that's a copy of the current instance, but
        /// with its <see cref="Link"/> value set to <paramref name="link"/>.
        /// </returns>
        public GpxPerson WithLink(GpxWebLink link) => new GpxPerson(Name, Email, link);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxPerson other &&
                                                   Name == other.Name &&
                                                   Equals(Email, other.Email) &&
                                                   Equals(Link, other.Link);

        /// <inheritdoc />
        public override int GetHashCode() => (Name, Email, Link).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(Name), Name),
                                                                 (nameof(Email), Email),
                                                                 (nameof(Link), Link));

        internal static GpxPerson Load(XElement element, bool allowOverlongDataUri)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxPerson(
                name: element.GpxElement("name")?.Value,
                email: GpxEmail.Load(element.GpxElement("email")),
                link: GpxWebLink.Load(element.GpxElement("link"), allowOverlongDataUri));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteOptionalGpxElementValue("name", Name);
            writer.WriteOptionalGpxElementValue("email", Email);
            writer.WriteOptionalGpxElementValue("link", Link);
        }
    }
}
