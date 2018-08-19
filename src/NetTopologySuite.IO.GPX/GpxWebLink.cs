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
        /// <param name="text">
        /// The value of <see cref="Text"/>.
        /// </param>
        /// <param name="contentType">
        /// The value of <see cref="ContentType"/>.
        /// </param>
        /// <param name="href">
        /// The value of <see cref="Href"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="href"/> is <see langword="null"/>.
        /// </exception>
        public GpxWebLink(string text, string contentType, Uri href)
        {
            this.Text = text;
            this.ContentType = contentType;
            this.Href = href ?? throw new ArgumentNullException(nameof(href));
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
        public override string ToString() => Helpers.BuildString((nameof(this.Text), this.Text),
                                                                 (nameof(this.ContentType), this.ContentType),
                                                                 (nameof(this.Href), this.Href));

        internal static GpxWebLink Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxWebLink(
                text: element.GpxElement("text")?.Value,
                contentType: element.GpxElement("type")?.Value,
                href: Helpers.ParseUri(element.Attribute("href")?.Value) ?? throw new XmlException("link element must have 'href' attribute"));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("href", this.Href.OriginalString);
            writer.WriteOptionalGpxElementValue("text", this.Text);
            writer.WriteOptionalGpxElementValue("type", this.ContentType);
        }
    }
}
