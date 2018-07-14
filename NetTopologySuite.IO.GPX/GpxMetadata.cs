using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxMetadata
    {
        public GpxMetadata(string name, string description, GpxPerson author, GpxCopyright copyright, ImmutableArray<GpxWebLink> webLinks, DateTime? creationTime, string keywords, GpxBoundingBox bounds, object extensions)
        {
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Copyright = copyright;
            this.WebLinks = webLinks.IsDefault ? ImmutableArray<GpxWebLink>.Empty : webLinks;
            this.CreationTime = creationTime;
            this.Keywords = keywords;
            this.Bounds = bounds;
            this.Extensions = extensions;
        }

        public static GpxMetadata Load(XElement element, GpxReaderSettings settings, Func<XElement, object> extensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (extensionCallback is null)
            {
                throw new ArgumentNullException(nameof(extensionCallback));
            }

            return new GpxMetadata(
                name: element.GpxElement("name")?.Value,
                description: element.GpxElement("desc")?.Value,
                author: GpxPerson.Load(element.GpxElement("author")),
                copyright: GpxCopyright.Load(element.GpxElement("copyright")),
                webLinks: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                creationTime: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo),
                keywords: element.GpxElement("keywords")?.Value,
                bounds: GpxBoundingBox.Load(element.GpxElement("bounds")),
                extensions: extensionCallback(element.GpxElement("extensions")));
        }

        public string Name { get; }

        public string Description { get; }

        public GpxPerson Author { get; }

        public GpxCopyright Copyright { get; }

        public ImmutableArray<GpxWebLink> WebLinks { get; }

        public DateTime? CreationTime { get; }

        public string Keywords { get; }

        public GpxBoundingBox Bounds { get; }

        public object Extensions { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.Name), this.Name),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Author), this.Author),
                                                                 (nameof(this.Copyright), this.Copyright),
                                                                 (nameof(this.WebLinks), Helpers.ListToString(this.WebLinks)),
                                                                 (nameof(this.CreationTime), this.CreationTime),
                                                                 (nameof(this.Keywords), this.Keywords),
                                                                 (nameof(this.Bounds), this.Bounds),
                                                                 (nameof(this.Extensions), this.Extensions));
    }
}
