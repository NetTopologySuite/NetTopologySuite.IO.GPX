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
        public GpxTrack()
        {
        }

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
            if (!links.IsDefault)
            {
                this.Links = links;
            }

            this.Number = number;
            this.Classification = classification;
            this.Extensions = extensions;
            if (!segments.IsDefault)
            {
                this.Segments = segments;
            }
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
        public ImmutableArray<GpxWebLink> Links { get; } = ImmutableArray<GpxWebLink>.Empty;

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
        public ImmutableArray<GpxTrackSegment> Segments { get; } = ImmutableArray<GpxTrackSegment>.Empty;

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Name"/> replaced by the given value.
        /// </summary>
        /// <param name="name">
        /// The new value for <see cref="Name"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Name"/> value set to <paramref name="name"/>.
        /// </returns>
        public GpxTrack WithName(string name) => new GpxTrack(name, this.Comment, this.Description, this.Source, this.Links, this.Number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Comment"/> replaced by the given value.
        /// </summary>
        /// <param name="comment">
        /// The new value for <see cref="Comment"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Comment"/> value set to <paramref name="comment"/>.
        /// </returns>
        public GpxTrack WithComment(string comment) => new GpxTrack(this.Name, comment, this.Description, this.Source, this.Links, this.Number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Description"/> replaced by the given value.
        /// </summary>
        /// <param name="description">
        /// The new value for <see cref="Description"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Description"/> value set to <paramref name="description"/>.
        /// </returns>
        public GpxTrack WithDescription(string description) => new GpxTrack(this.Name, this.Comment, description, this.Source, this.Links, this.Number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Source"/> replaced by the given value.
        /// </summary>
        /// <param name="source">
        /// The new value for <see cref="Source"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Source"/> value set to <paramref name="source"/>.
        /// </returns>
        public GpxTrack WithSource(string source) => new GpxTrack(this.Name, this.Comment, this.Description, source, this.Links, this.Number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Links"/> replaced by the given value.
        /// </summary>
        /// <param name="links">
        /// The new value for <see cref="Links"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Links"/> value set to <paramref name="links"/>.
        /// </returns>
        public GpxTrack WithLinks(ImmutableArray<GpxWebLink> links) => new GpxTrack(this.Name, this.Comment, this.Description, this.Source, links, this.Number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Number"/> replaced by the given value.
        /// </summary>
        /// <param name="number">
        /// The new value for <see cref="Number"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Number"/> value set to <paramref name="number"/>.
        /// </returns>
        public GpxTrack WithNumber(uint? number) => new GpxTrack(this.Name, this.Comment, this.Description, this.Source, this.Links, number, this.Classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Classification"/> replaced by the given value.
        /// </summary>
        /// <param name="classification">
        /// The new value for <see cref="Classification"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Classification"/> value set to <paramref name="classification"/>.
        /// </returns>
        public GpxTrack WithClassification(string classification) => new GpxTrack(this.Name, this.Comment, this.Description, this.Source, this.Links, this.Number, classification, this.Extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Extensions"/> replaced by the given value.
        /// </summary>
        /// <param name="extensions">
        /// The new value for <see cref="Extensions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Extensions"/> value set to <paramref name="extensions"/>.
        /// </returns>
        public GpxTrack WithExtensions(object extensions) => new GpxTrack(this.Name, this.Comment, this.Description, this.Source, this.Links, this.Number, this.Classification, extensions, this.Segments);

        /// <summary>
        /// Builds a new instance of <see cref="GpxTrack"/> as a copy of this instance, but with
        /// <see cref="Segments"/> replaced by the given value.
        /// </summary>
        /// <param name="segments">
        /// The new value for <see cref="Segments"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxTrack"/> instance that's a copy of the current instance, but
        /// with its <see cref="Segments"/> value set to <paramref name="segments"/>.
        /// </returns>
        public GpxTrack WithSegments(ImmutableArray<GpxTrackSegment> segments) => new GpxTrack(this.Name, this.Comment, this.Description, this.Source, this.Links, this.Number, this.Classification, this.Extensions, segments);

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
