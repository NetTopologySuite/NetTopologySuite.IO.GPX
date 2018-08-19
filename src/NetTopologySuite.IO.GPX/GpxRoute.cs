using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a route - an ordered list of waypoints representing a series of turn points
    /// leading to a destination.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_rteType">rteType</a>".
    /// </remarks>
    public sealed class GpxRoute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxRoute"/> class.
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
        /// <param name="waypoints">
        /// The value for <see cref="Waypoints"/>.
        /// </param>
        public GpxRoute(string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, uint? number, string classification, object extensions, ImmutableGpxWaypointTable waypoints)
        {
            this.Name = name;
            this.Comment = comment;
            this.Description = description;
            this.Source = source;
            this.Links = links.IsDefault ? ImmutableArray<GpxWebLink>.Empty : links;
            this.Number = number;
            this.Classification = classification;
            this.Waypoints = waypoints;
            this.Extensions = extensions;
        }

        /// <summary>
        /// Get the GPS name of this route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "name" element.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the GPS comment for this route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "cmt" element.
        /// </remarks>
        public string Comment { get; }

        /// <summary>
        /// Gets the text description for this route (not sent to GPS).
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
        /// Gets links to external information about the route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" elements.
        /// </remarks>
        public ImmutableArray<GpxWebLink> Links { get; }

        /// <summary>
        /// Gets the GPS route number.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "number" element.
        /// </remarks>
        public uint? Number { get; }

        /// <summary>
        /// Gets the classification of the route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "type" element.
        /// </remarks>
        public string Classification { get; }

        /// <summary>
        /// Gets arbitrary extension information associated with this route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </remarks>
        public object Extensions { get; }

        /// <summary>
        /// Gets the waypoints that make up this route.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "rtept" elements.
        /// </remarks>
        public ImmutableGpxWaypointTable Waypoints { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Name), this.Name),
                                                                 (nameof(this.Comment), this.Comment),
                                                                 (nameof(this.Description), this.Description),
                                                                 (nameof(this.Source), this.Source),
                                                                 (nameof(this.Links), Helpers.ListToString(this.Links)),
                                                                 (nameof(this.Number), this.Number),
                                                                 (nameof(this.Classification), this.Classification),
                                                                 (nameof(this.Extensions), this.Extensions),
                                                                 (nameof(this.Waypoints), Helpers.BuildString((nameof(this.Waypoints.Count), this.Waypoints.Count))));

        internal static GpxRoute Load(XElement element, GpxReaderSettings settings)
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
            return new GpxRoute(
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                number: Helpers.ParseUInt32(element.GpxElement("number")?.Value),
                classification: element.GpxElement("type")?.Value,
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertRouteExtensionElement(extensionsElement.Elements()),
                waypoints: new ImmutableGpxWaypointTable(element.GpxElements("rtept"), settings, settings.ExtensionReader.ConvertRoutePointExtensionElement));
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
            writer.WriteExtensions(this.Extensions, settings.ExtensionWriter.ConvertRouteExtension);
            Func<object, IEnumerable<XElement>> extensionCallback = settings.ExtensionWriter.ConvertRoutePointExtension;
            foreach (var waypoint in this.Waypoints)
            {
                writer.WriteGpxStartElement("rtept");
                waypoint.Save(writer, settings, extensionCallback);
                writer.WriteEndElement();
            }
        }
    }
}
