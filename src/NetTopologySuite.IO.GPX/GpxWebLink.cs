using System;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A link to an external resource (Web page, digital photo, video clip, etc) with additional
    /// information.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_linkType">linkType</a>".
    /// </remarks>
    public sealed class GpxWebLink : ICanWriteToXmlWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWebLink"/> class.
        /// </summary>
        /// <param name="href">
        /// The value of <see cref="Href"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="href"/> is <see langword="null"/>.
        /// </exception>
        public GpxWebLink(Uri href)
            : this(href, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWebLink"/> class.
        /// </summary>
        /// <param name="href">
        /// The value of <see cref="Href"/>.
        /// </param>
        /// <param name="text">
        /// The value of <see cref="Text"/>.
        /// </param>
        /// <param name="contentType">
        /// The value of <see cref="ContentType"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="href"/> is <see langword="null"/>.
        /// </exception>
        public GpxWebLink(Uri href, string text, string contentType)
        {
            this.Href = href ?? throw new ArgumentNullException(nameof(href));
            this.Text = text;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets the text of the hyperlink.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "text" element.
        /// </remarks>
        public string Text { get; }

        /// <summary>
        /// Gets the MIME type of content (e.g., image/jpeg).
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "type" element.
        /// </remarks>
        public string ContentType { get; }

        /// <summary>
        /// Gets the URL of the hyperlink.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "href" attribute.
        /// </remarks>
        public Uri Href { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxWebLink other &&
                                                   this.Text == other.Text &&
                                                   this.ContentType == other.ContentType &&
                                                   this.Href == other.Href;

        /// <inheritdoc />
        public override int GetHashCode() => (this.Href, this.Text, this.ContentType).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Href), this.Href),
                                                                 (nameof(this.Text), this.Text),
                                                                 (nameof(this.ContentType), this.ContentType));

        /// <summary>
        /// Builds a new instance of <see cref="GpxWebLink"/> as a copy of this instance, but with
        /// <see cref="Text"/> replaced by the given value.
        /// </summary>
        /// <param name="text">
        /// The new value for <see cref="Text"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWebLink"/> instance that's a copy of the current instance, but
        /// with its <see cref="Text"/> value set to <paramref name="text"/>.
        /// </returns>
        public GpxWebLink WithText(string text) => new GpxWebLink(this.Href, text, this.ContentType);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWebLink"/> as a copy of this instance, but with
        /// <see cref="ContentType"/> replaced by the given value.
        /// </summary>
        /// <param name="contentType">
        /// The new value for <see cref="ContentType"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWebLink"/> instance that's a copy of the current instance, but
        /// with its <see cref="ContentType"/> value set to <paramref name="contentType"/>.
        /// </returns>
        public GpxWebLink WithContentType(string contentType) => new GpxWebLink(this.Href, this.Text, contentType);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWebLink"/> as a copy of this instance, but with
        /// <see cref="Href"/> replaced by the given value.
        /// </summary>
        /// <param name="href">
        /// The new value for <see cref="Href"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWebLink"/> instance that's a copy of the current instance, but
        /// with its <see cref="Href"/> value set to <paramref name="href"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="href"/> is <see langword="null"/>.
        /// </exception>
        public GpxWebLink WithHref(Uri href) => new GpxWebLink(href, this.Text, this.ContentType);

        internal static GpxWebLink Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxWebLink(
                href: Helpers.ParseUri(element.Attribute("href")?.Value) ?? throw new XmlException("link element must have 'href' attribute"),
                text: element.GpxElement("text")?.Value,
                contentType: element.GpxElement("type")?.Value);
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("href", this.Href.OriginalString);
            writer.WriteOptionalGpxElementValue("text", this.Text);
            writer.WriteOptionalGpxElementValue("type", this.ContentType);
        }
    }
}
