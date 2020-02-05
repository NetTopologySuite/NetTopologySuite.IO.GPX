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
        public GpxRoute()
        {
        }

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
            Name = name;
            Comment = comment;
            Description = description;
            Source = source;
            if (!links.IsDefault)
            {
                Links = links;
            }

            Number = number;
            Classification = classification;
            Extensions = extensions;
            if (!(waypoints is null))
            {
                Waypoints = waypoints;
            }
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
        public ImmutableArray<GpxWebLink> Links { get; } = ImmutableArray<GpxWebLink>.Empty;

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
        public ImmutableGpxWaypointTable Waypoints { get; } = ImmutableGpxWaypointTable.Empty;

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Name"/> replaced by the given value.
        /// </summary>
        /// <param name="name">
        /// The new value for <see cref="Name"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Name"/> value set to <paramref name="name"/>.
        /// </returns>
        public GpxRoute WithName(string name) => new GpxRoute(name, Comment, Description, Source, Links, Number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Comment"/> replaced by the given value.
        /// </summary>
        /// <param name="comment">
        /// The new value for <see cref="Comment"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Comment"/> value set to <paramref name="comment"/>.
        /// </returns>
        public GpxRoute WithComment(string comment) => new GpxRoute(Name, comment, Description, Source, Links, Number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Description"/> replaced by the given value.
        /// </summary>
        /// <param name="description">
        /// The new value for <see cref="Description"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Description"/> value set to <paramref name="description"/>.
        /// </returns>
        public GpxRoute WithDescription(string description) => new GpxRoute(Name, Comment, description, Source, Links, Number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Source"/> replaced by the given value.
        /// </summary>
        /// <param name="source">
        /// The new value for <see cref="Source"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Source"/> value set to <paramref name="source"/>.
        /// </returns>
        public GpxRoute WithSource(string source) => new GpxRoute(Name, Comment, Description, source, Links, Number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Links"/> replaced by the given value.
        /// </summary>
        /// <param name="links">
        /// The new value for <see cref="Links"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Links"/> value set to <paramref name="links"/>.
        /// </returns>
        public GpxRoute WithLinks(ImmutableArray<GpxWebLink> links) => new GpxRoute(Name, Comment, Description, Source, links, Number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Number"/> replaced by the given value.
        /// </summary>
        /// <param name="number">
        /// The new value for <see cref="Number"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Number"/> value set to <paramref name="number"/>.
        /// </returns>
        public GpxRoute WithNumber(uint? number) => new GpxRoute(Name, Comment, Description, Source, Links, number, Classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Classification"/> replaced by the given value.
        /// </summary>
        /// <param name="classification">
        /// The new value for <see cref="Classification"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Classification"/> value set to <paramref name="classification"/>.
        /// </returns>
        public GpxRoute WithClassification(string classification) => new GpxRoute(Name, Comment, Description, Source, Links, Number, classification, Extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Extensions"/> replaced by the given value.
        /// </summary>
        /// <param name="extensions">
        /// The new value for <see cref="Extensions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Extensions"/> value set to <paramref name="extensions"/>.
        /// </returns>
        public GpxRoute WithExtensions(object extensions) => new GpxRoute(Name, Comment, Description, Source, Links, Number, Classification, extensions, Waypoints);

        /// <summary>
        /// Builds a new instance of <see cref="GpxRoute"/> as a copy of this instance, but with
        /// <see cref="Waypoints"/> replaced by the given value.
        /// </summary>
        /// <param name="waypoints">
        /// The new value for <see cref="Waypoints"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxRoute"/> instance that's a copy of the current instance, but
        /// with its <see cref="Waypoints"/> value set to <paramref name="waypoints"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="waypoints"/> contains <see langword="null"/> elements.
        /// </exception>
        public GpxRoute WithWaypoints(IEnumerable<GpxWaypoint> waypoints) => new GpxRoute(Name, Comment, Description, Source, Links, Number, Classification, Extensions, new ImmutableGpxWaypointTable(waypoints ?? ImmutableGpxWaypointTable.Empty));

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxRoute other &&
                                                   Name == other.Name &&
                                                   Comment == other.Comment &&
                                                   Description == other.Description &&
                                                   Source == other.Source &&
                                                   Links.ListEquals(other.Links) &&
                                                   Number == other.Number &&
                                                   Classification == other.Classification &&
                                                   Equals(Extensions, other.Extensions) &&
                                                   Equals(Waypoints, other.Waypoints);

        /// <inheritdoc />
        public override int GetHashCode() => (Name, Comment, Description, Source, Links.ListToHashCode(), Number, Classification, Extensions, Waypoints).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(Name), Name),
                                                                 (nameof(Comment), Comment),
                                                                 (nameof(Description), Description),
                                                                 (nameof(Source), Source),
                                                                 (nameof(Links), Helpers.ListToString(Links)),
                                                                 (nameof(Number), Number),
                                                                 (nameof(Classification), Classification),
                                                                 (nameof(Extensions), Extensions),
                                                                 (nameof(Waypoints), Helpers.BuildString((nameof(Waypoints.Count), Waypoints.Count))));

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
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(el => GpxWebLink.Load(el, settings.BuildWebLinksForVeryLongUriValues))),
                number: Helpers.ParseUInt32(element.GpxElement("number")?.Value),
                classification: element.GpxElement("type")?.Value,
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertRouteExtensionElement(extensionsElement.Elements()),
                waypoints: new ImmutableGpxWaypointTable(element.GpxElements("rtept"), settings, settings.ExtensionReader.ConvertRoutePointExtensionElement));
        }

        internal void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            writer.WriteOptionalGpxElementValue("name", Name);
            writer.WriteOptionalGpxElementValue("cmt", Comment);
            writer.WriteOptionalGpxElementValue("desc", Description);
            writer.WriteOptionalGpxElementValue("src", Source);
            writer.WriteGpxElementValues("link", Links);
            writer.WriteOptionalGpxElementValue("number", Number);
            writer.WriteOptionalGpxElementValue("type", Classification);
            writer.WriteExtensions(Extensions, settings.ExtensionWriter.ConvertRouteExtension);
            Func<object, IEnumerable<XElement>> extensionCallback = settings.ExtensionWriter.ConvertRoutePointExtension;
            foreach (var waypoint in Waypoints)
            {
                writer.WriteGpxStartElement("rtept");
                waypoint.Save(writer, settings, extensionCallback);
                writer.WriteEndElement();
            }
        }
    }
}
