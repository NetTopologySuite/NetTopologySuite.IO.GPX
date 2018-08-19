using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a track - an ordered list of points describing a path.  Similar to a
    /// <see cref="GpxRoute"/>, but separated into <see cref="GpxTrackSegment"/>s at points that may
    /// indicate GPS reception temporarily dropping off or the receiver being turned off.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_trkType">trkType</a>".
    /// </remarks>
    public sealed class GpxTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxTrack"/> class.
        /// </summary>
        /// <param name="name">
        /// The value for <see cref="Name"/>.
        /// </param>
        /// <param name="comment">
        /// The value for <see cref="Comment"/>.
        /// </param>
        /// <param name="description">
        /// The value for <see cref="Description"/>.
        /// </param>
        /// <param name="source">
        /// The value for <see cref="Source"/>.
        /// </param>
        /// <param name="links">
        /// The value for <see cref="Links"/>.
        /// </param>
        /// <param name="number">
        /// The value for <see cref="Number"/>.
        /// </param>
        /// <param name="classification">
        /// The value for <see cref="Classification"/>.
        /// </param>
        /// <param name="extensions">
        /// The value for <see cref="Extensions"/>.
        /// </param>
        /// <param name="segments">
        /// The value for <see cref="Segments"/>.
        /// </param>
        public GpxTrack(string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, uint? number, string classification, object extensions, ImmutableArray<GpxTrackSegment> segments)
        {
            this.Name = name;
            this.Comment = comment;
            this.Description = description;
            this.Source = source;
            this.Links = links.IsDefault ? ImmutableArray<GpxWebLink>.Empty : links;
            this.Number = number;
            this.Classification = classification;
            this.Segments = segments;
            this.Extensions = extensions;
        }

        /// <summary>
        /// Get the GPS name of this track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "name" element.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the GPS comment for this track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "cmt" element.
        /// </remarks>
        public string Comment { get; }

        /// <summary>
        /// Gets the user description for this track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "desc" element.
        /// </remarks>
        public string Description { get; }

        /// <summary>
        /// Gets the source of the data.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "src" element.
        /// </remarks>
        public string Source { get; }

        /// <summary>
        /// Gets links to external information about the track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" elements.
        /// </remarks>
        public ImmutableArray<GpxWebLink> Links { get; }

        /// <summary>
        /// Gets the GPS track number.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "number" element.
        /// </remarks>
        public uint? Number { get; }

        /// <summary>
        /// Gets the classification of the track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "type" element.
        /// </remarks>
        public string Classification { get; }

        /// <summary>
        /// Gets arbitrary extension information associated with this track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions { get; }

        /// <summary>
        /// Gets the segments that make up this track.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "trkseg" elements.
        /// </remarks>
        public ImmutableArray<GpxTrackSegment> Segments { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Name), this.Name),
                                                                 (nameof(this.Comment), this.Comment),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Source), this.Source),
                                                                 (nameof(this.Links), Helpers.ListToString(this.Links)),
                                                                 (nameof(this.Number), this.Number),
                                                                 (nameof(this.Classification), this.Classification),
                                                                 (nameof(this.Extensions), this.Extensions),
                                                                 (nameof(this.Segments), Helpers.BuildString((nameof(this.Segments.Length), this.Segments.Length))));

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
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertTrackExtensionElement(extensionsElement.Elements()),
                segments: ImmutableArray.CreateRange(element.GpxElements("trkseg").Select(el => GpxTrackSegment.Load(el, settings))));
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
