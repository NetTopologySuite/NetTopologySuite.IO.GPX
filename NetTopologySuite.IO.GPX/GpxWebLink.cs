using System;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A link to an external resource (Web page, digital photo, video clip, etc) with additional
    /// information.
    /// </summary>
    public sealed class GpxWebLink : ICanWriteToXmlWriter
    {
        public GpxWebLink(string text, string contentType, Uri href)
        {
            this.Text = text;
            this.ContentType = contentType;
            this.Href = href ?? throw new ArgumentNullException(nameof(href));
        }

        public static GpxWebLink Load(XElement element)
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

        public void Save(XmlWriter writer)
        {
            writer.WriteAttributeString("href", this.Href.OriginalString);
            writer.WriteOptionalGpxElementValue("text", this.Text);
            writer.WriteOptionalGpxElementValue("type", this.ContentType);
        }

        public string Text { get; }

        public string ContentType { get; }

        public Uri Href { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.Text), this.Text),
                                                                 (nameof(this.ContentType), this.ContentType),
                                                                 (nameof(this.Href), this.Href));
    }
}
