using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxTrack
    {
        public GpxTrack(string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, uint? number, string classification, ImmutableArray<GpxTrackSegment> segments, object extensions)
        {
            this.Name = name;
            this.Comment = comment;
            this.Description = description;
            this.Source = source;
            this.Links = links;
            this.Number = number;
            this.Classification = classification;
            this.Segments = segments;
            this.Extensions = extensions;
        }

        public string Name { get; }

        public string Comment { get; }

        public string Description { get; }

        public string Source { get; }

        public ImmutableArray<GpxWebLink> Links { get; }

        public uint? Number { get; }

        public string Classification { get; }

        public ImmutableArray<GpxTrackSegment> Segments { get; }

        public object Extensions { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Name), this.Name),
                                                                 (nameof(this.Comment), this.Comment),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Source), this.Source),
                                                                 (nameof(this.Links), Helpers.ListToString(this.Links)),
                                                                 (nameof(this.Number), this.Number),
                                                                 (nameof(this.Classification), this.Classification),
                                                                 (nameof(this.Segments), Helpers.BuildString((nameof(this.Segments.Length), this.Segments.Length))),
                                                                 (nameof(this.Extensions), this.Extensions));

        internal static GpxTrack Load(XElement element, GpxReaderSettings settings)
        {
            if (element is null)
            {
                return null;
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var extensionsElement = element.GpxElement("extensions");
            return new GpxTrack(
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                number: Helpers.ParseUInt32(element.GpxElement("number")?.Value),
                classification: element.GpxElement("type")?.Value,
                segments: ImmutableArray.CreateRange(element.GpxElements("trkseg").Select(el => GpxTrackSegment.Load(el, settings))),
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertTrackExtensionElement(extensionsElement.Elements()));
        }

        internal void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            writer.WriteOptionalGpxElementValue("name", this.Name);
            writer.WriteOptionalGpxElementValue("cmt", this.Comment);
            writer.WriteOptionalGpxElementValue("desc", this.Description);
            writer.WriteOptionalGpxElementValue("src", this.Source);
            writer.WriteGpxElementValues("link", this.Links);
            writer.WriteOptionalGpxElementValue("number", this.Number);
            writer.WriteOptionalGpxElementValue("type", this.Classification);
            writer.WriteExtensions(this.Extensions, settings.ExtensionWriter.ConvertTrackExtension);
            foreach (var segment in this.Segments)
            {
                writer.WriteGpxStartElement("trkseg");
                segment.Save(writer, settings);
                writer.WriteEndElement();
            }
        }
    }
}
