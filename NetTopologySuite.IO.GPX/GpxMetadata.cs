using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxMetadata
    {
        public GpxMetadata(string creator)
            : this(creator, default, default, default, default, default, default, default, default, default)
        {
        }

        public GpxMetadata(string creator, string name, string description, GpxPerson author, GpxCopyright copyright, ImmutableArray<GpxWebLink> links, DateTime? creationTime, string keywords, GpxBoundingBox bounds, object extensions)
        {
            this.Creator = creator;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Copyright = copyright;
            this.Links = links.IsDefault ? ImmutableArray<GpxWebLink>.Empty : links;
            this.CreationTime = creationTime;
            this.Keywords = keywords;
            this.Bounds = bounds;
            this.Extensions = extensions;
            this.IsTrivial = name is null && description is null && author is null && copyright is null && links.IsDefaultOrEmpty && creationTime is null && keywords is null && bounds is null && extensions is null;
        }

        public static GpxMetadata Load(XElement element, GpxReaderSettings settings, string creator)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            var extensionsElement = element.GpxElement("extensions");
            return new GpxMetadata(
                creator: creator,
                name: element.GpxElement("name")?.Value,
                description: element.GpxElement("desc")?.Value,
                author: GpxPerson.Load(element.GpxElement("author")),
                copyright: GpxCopyright.Load(element.GpxElement("copyright")),
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                creationTime: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo),
                keywords: element.GpxElement("keywords")?.Value,
                bounds: GpxBoundingBox.Load(element.GpxElement("bounds")),
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertMetadataExtensionElement(extensionsElement.Elements()));
        }

        public void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            // caller wrote Creator (it's an attribute on the root tag)
            writer.WriteOptionalGpxElementValue("name", this.Name);
            writer.WriteOptionalGpxElementValue("desc", this.Description);
            writer.WriteOptionalGpxElementValue("author", this.Author);
            writer.WriteOptionalGpxElementValue("copyright", this.Copyright);
            writer.WriteGpxElementValues("link", this.Links);
            writer.WriteOptionalGpxElementValue("time", this.CreationTime);
            writer.WriteOptionalGpxElementValue("keywords", this.Keywords);
            writer.WriteOptionalGpxElementValue("bounds", this.Bounds);
            writer.WriteExtensions(this.Extensions, settings.ExtensionWriter.ConvertMetadataExtension);
        }

        public bool IsTrivial { get; }

        public string Creator { get; }

        public string Name { get; }

        public string Description { get; }

        public GpxPerson Author { get; }

        public GpxCopyright Copyright { get; }

        public ImmutableArray<GpxWebLink> Links { get; }

        public DateTime? CreationTime { get; }

        public string Keywords { get; }

        public GpxBoundingBox Bounds { get; }

        public object Extensions { get; }

        public override string ToString() => Helpers.BuildString((nameof(this.Creator), this.Creator),
                                                                 (nameof(this.Name), this.Name),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Author), this.Author),
                                                                 (nameof(this.Copyright), this.Copyright),
                                                                 (nameof(this.Links), Helpers.ListToString(this.Links)),
                                                                 (nameof(this.CreationTime), this.CreationTime),
                                                                 (nameof(this.Keywords), this.Keywords),
                                                                 (nameof(this.Bounds), this.Bounds),
                                                                 (nameof(this.Extensions), this.Extensions));
    }
}
