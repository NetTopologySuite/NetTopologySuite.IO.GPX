using System;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxCopyright
    {
        public GpxCopyright(GregorianYearWithOptionalOffset? year, Uri licenseUri, string author)
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
                year: Helpers.ParseGregorianYearWithOptionalOffset(element.GpxElement("year")?.Value),
                licenseUri: Helpers.ParseUri(element.GpxElement("license")?.Value),
                author: element.GpxAttribute("author")?.Value ?? throw new XmlException("copyright element must have author attribute."));
        }

        public GregorianYearWithOptionalOffset? Year { get; }

        public Uri LicenseUri { get; }

        public string Author { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.Year), this.Year),
                                                                 (nameof(this.LicenseUri), this.LicenseUri),
                                                                 (nameof(this.Author), this.Author));
    }
}
