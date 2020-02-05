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
            Href = href ?? throw new ArgumentNullException(nameof(href));
            HrefString = href.OriginalString;
            Text = text;
            ContentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWebLink"/> class.
        /// <para>
        /// If <paramref name="hrefString"/> is never longer than 65519 characters, then favor using
        /// <see cref="GpxWebLink(Uri, string, string)"/> instead of this, since <see cref="Href"/>
        /// will never be <see langword="null"/> when using that constructor.
        /// </para>
        /// </summary>
        /// <param name="hrefString">
        /// The value of <see cref="HrefString"/>.
        /// </param>
        /// <param name="text">
        /// The value of <see cref="Text"/>.
        /// </param>
        /// <param name="contentType">
        /// The value of <see cref="ContentType"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hrefString"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="hrefString"/> does not look like a valid URI string.
        /// </exception>
        public GpxWebLink(string hrefString, string text, string contentType)
        {
            switch (Helpers.InterpretUri(hrefString, out var bestEffortHrefUri))
            {
                case UriValidationResult.NullValue:
                    throw new ArgumentNullException(nameof(hrefString));

                case UriValidationResult.ValidSystemUri:
                    Href = bestEffortHrefUri;
                    break;

                case UriValidationResult.ValidOverlongDataUri:
                    break;

                default:
                    throw new ArgumentException("does not look like a valid URI string", nameof(hrefString));
            }

            HrefString = hrefString;
            Text = text;
            ContentType = contentType;
        }

        private GpxWebLink(string hrefString, string text, string contentType, Uri bestEffortHrefUri)
        {
            HrefString = hrefString;
            Text = text;
            ContentType = contentType;
            Href = bestEffortHrefUri;
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
        /// Gets the URL of the hyperlink, or <see langword="null"/> if this instance was created
        /// from a valid-looking data URI whose total length exceeded 65519 characters.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "href" attribute.
        /// </remarks>
        public Uri Href { get; }

        /// <summary>
        /// Gets the URL of the hyperlink, as a string.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "href" attribute.
        /// </remarks>
        public string HrefString { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxWebLink other &&
                                                   Text == other.Text &&
                                                   ContentType == other.ContentType &&
                                                   HrefString == other.HrefString;

        /// <inheritdoc />
        public override int GetHashCode() => (HrefString, Text, ContentType).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(HrefString), HrefString),
                                                                 (nameof(Text), Text),
                                                                 (nameof(ContentType), ContentType));

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
        public GpxWebLink WithText(string text) => new GpxWebLink(Href, text, ContentType);

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
        public GpxWebLink WithContentType(string contentType) => new GpxWebLink(Href, Text, contentType);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWebLink"/> as a copy of this instance, but with
        /// <see cref="Href"/> and <see cref="HrefString"/> replaced by the given value.
        /// </summary>
        /// <param name="href">
        /// The new value for <see cref="Href"/> and <see cref="HrefString"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWebLink"/> instance that's a copy of the current instance, but
        /// with its <see cref="Href"/> value set to <paramref name="href"/> and with its
        /// <see cref="HrefString"/> set accordingly.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="href"/> is <see langword="null"/>.
        /// </exception>
        public GpxWebLink WithHref(Uri href) => new GpxWebLink(href, Text, ContentType);

        /// <summary>
        /// Builds a new instance of <see cref="GpxWebLink"/> as a copy of this instance, but with
        /// <see cref="HrefString"/> and <see cref="Href"/> replaced according to the given value.
        /// </summary>
        /// <param name="hrefString">
        /// The new value for <see cref="HrefString"/> and (if possible) <see cref="Href"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxWebLink"/> instance that's a copy of the current instance, but
        /// with its <see cref="HrefString"/> value set to <paramref name="hrefString"/> and its
        /// <see cref="Href"/> value set accordingly.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hrefString"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="hrefString"/> does not look like a valid URI string.
        /// </exception>
        public GpxWebLink WithHrefString(string hrefString) => new GpxWebLink(hrefString, Text, ContentType);

        internal static GpxWebLink Load(XElement element, bool allowOverlongDataUri)
        {
            if (element is null)
            {
                return null;
            }

            string hrefString = element.Attribute("href")?.Value;
            string text = element.GpxElement("text")?.Value;
            string contentType = element.GpxElement("type")?.Value;
            switch (Helpers.InterpretUri(hrefString, out var bestEffortHrefUri))
            {
                case UriValidationResult.NullValue:
                    throw new XmlException("link element must have 'href' attribute");

                case UriValidationResult.ValidSystemUri:
                    return new GpxWebLink(
                        href: bestEffortHrefUri,
                        text: text,
                        contentType: contentType);

                case UriValidationResult.ValidOverlongDataUri:
                    if (!allowOverlongDataUri)
                    {
                        throw new XmlException($"link element's 'href' attribute looks like a valid (but long) data URI.  GpxWebLink.Href will be null in these cases, so to allow this, you will need to set the {nameof(GpxReaderSettings.BuildWebLinksForVeryLongUriValues)} flag.");
                    }

                    return new GpxWebLink(
                        hrefString: hrefString,
                        text: text,
                        contentType: contentType,
                        bestEffortHrefUri: bestEffortHrefUri);

                default:
                    throw new XmlException("link element's 'href' attribute does not look like a valid URI");
            }
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("href", HrefString);
            writer.WriteOptionalGpxElementValue("text", Text);
            writer.WriteOptionalGpxElementValue("type", ContentType);
        }
    }
}
