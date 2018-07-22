using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxCopyright : ICanWriteToXmlWriter
    {
        public GpxCopyright(int? year, Uri licenseUri, string author)
        {
            this.Year = year;
            this.LicenseUri = licenseUri;
            this.Author = author ?? throw new ArgumentNullException(nameof(author));
        }

        public static GpxCopyright Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxCopyright(
                year: Helpers.ParseGregorianYear(element.GpxElement("year")?.Value),
                licenseUri: Helpers.ParseUri(element.GpxElement("license")?.Value),
                author: element.Attribute("author")?.Value ?? throw new XmlException("copyright element must have author attribute."));
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteAttributeString("author", this.Author);
            writer.WriteOptionalGpxElementValue("year", this.Year?.ToString("0000", CultureInfo.InvariantCulture));
        }

        public int? Year { get; }

        public Uri LicenseUri { get; }

        public string Author { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.Year), this.Year),
                                                                 (nameof(this.LicenseUri), this.LicenseUri),
                                                                 (nameof(this.Author), this.Author));
    }
}
