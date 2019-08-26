using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents metadata about a GPX document.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_metadataType">metadataType</a>",
    /// plus the "creator" attribute from the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_gpxType">gpxType</a>".
    /// </remarks>
    public sealed class GpxMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxMetadata"/> class.
        /// </summary>
        /// <param name="creator">
        /// The value for <see cref="Creator"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="creator"/> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <see cref="IsTrivial"/> will always be <see langword="true"/> for instances initialized
        /// using this constructor.
        /// </remarks>
        public GpxMetadata(string creator)
            : this(creator, default, default, default, default, default, default, default, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxMetadata"/> class.
        /// </summary>
        /// <param name="creator">
        /// The value for <see cref="Creator"/>.
        /// </param>
        /// <param name="name">
        /// The value for <see cref="Name"/>.
        /// </param>
        /// <param name="description">
        /// The value for <see cref="Description"/>.
        /// </param>
        /// <param name="author">
        /// The value for <see cref="Author"/>.
        /// </param>
        /// <param name="copyright">
        /// The value for <see cref="Copyright"/>.
        /// </param>
        /// <param name="links">
        /// The value for <see cref="Links"/>.
        /// </param>
        /// <param name="creationTimeUtc">
        /// The value for <see cref="CreationTimeUtc"/>.
        /// </param>
        /// <param name="keywords">
        /// The value for <see cref="Keywords"/>.
        /// </param>
        /// <param name="bounds">
        /// The value for <see cref="Bounds"/>.
        /// </param>
        /// <param name="extensions">
        /// The value for <see cref="Extensions"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="creator"/> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <see cref="IsTrivial"/> will be <see langword="false"/> for instances initialized using
        /// this constructor, unless <paramref name="links"/> contains no elements and all other
        /// parameters besides <paramref name="creator"/> are <see langword="null"/>.
        /// </remarks>
        public GpxMetadata(string creator, string name, string description, GpxPerson author, GpxCopyright copyright, ImmutableArray<GpxWebLink> links, DateTime? creationTimeUtc, string keywords, GpxBoundingBox bounds, object extensions)
        {
            if (creationTimeUtc.HasValue && creationTimeUtc.GetValueOrDefault().Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Must be in UTC.", nameof(creationTimeUtc));
            }

            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            Name = name;
            Description = description;
            Author = author;
            Copyright = copyright;
            if (!links.IsDefault)
            {
                Links = links;
            }

            CreationTimeUtc = creationTimeUtc;
            Keywords = keywords;
            Bounds = bounds;
            Extensions = extensions;
            IsTrivial = name is null && description is null && author is null && copyright is null && links.IsDefaultOrEmpty && creationTimeUtc is null && keywords is null && bounds is null && extensions is null;
        }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="Creator"/> is the only relevant data
        /// element.  Used to determine when we can skip writing out a "metadata" element.
        /// </summary>
        /// <remarks>
        /// This behavior is not completely round-trip safe.  An instance loaded from XML that has a
        /// bare "metadata" element without any content will look the same as an instance that had
        /// no "metadata" element at all.  It will be written out without a "metadata" element.
        /// </remarks>
        public bool IsTrivial { get; }

        /// <summary>
        /// Gets the creator of this GPX document.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "creator" attribute of
        /// the top-level "gpx" element.
        /// </remarks>
        public string Creator { get; }

        /// <summary>
        /// Gets an optional name for this GPX file.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "name" element.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets an optional description of the contents of this GPX file.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "desc" element.
        /// </remarks>
        public string Description { get; }

        /// <summary>
        /// Gets an optional representation of the person who created this GPX file.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "author" element.
        /// </remarks>
        public GpxPerson Author { get; }

        /// <summary>
        /// Gets an optional representation of the copyright that this GPX file was created under.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "copyright" element.
        /// </remarks>
        public GpxCopyright Copyright { get; }

        /// <summary>
        /// Gets a representation of URLs associated with the contents of this GPX file.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "link" elements.
        /// </remarks>
        public ImmutableArray<GpxWebLink> Links { get; } = ImmutableArray<GpxWebLink>.Empty;

        /// <summary>
        /// Gets an optional timestamp indicating when this GPX file was created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "time" element.
        /// </para>
        /// <para>
        /// The value, if present, will have its <see cref="DateTime.Kind"/> set to
        /// <see cref="DateTimeKind.Utc"/>.
        /// </para>
        /// </remarks>
        public DateTime? CreationTimeUtc { get; }

        /// <summary>
        /// Gets an optional list of keywords associated with this GPX file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "keywords" element.
        /// </para>
        /// <para>
        /// GPX does not define, by construction, how this string should be built when multiple
        /// keywords exist, so check with your provider / consumer what convention to use.
        /// </para>
        /// </remarks>
        public string Keywords { get; }

        /// <summary>
        /// Gets an optional representation of the lat/lon extents covered by this GPX file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "bounds" element.
        /// </para>
        /// <para>
        /// The system does not currently validate that this value is an accurate representation of
        /// the extents of the contents of a GPX file.
        /// </para>
        /// </remarks>
        public GpxBoundingBox Bounds { get; }

        /// <summary>
        /// Gets an optional representation of arbitrary data associated with the metadata of this GPX file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "extensions" element.
        /// </para>
        /// <para>
        /// This contains the extension content of the "metadata" element itself, <b>not</b> the
        /// extension content of the outermost "gpx" element.
        /// </para>
        /// </remarks>
        /// <seealso cref="GpxExtensionReader.ConvertMetadataExtensionElement"/>
        /// <seealso cref="GpxExtensionWriter.ConvertMetadataExtension"/>
        public object Extensions { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxMetadata other &&
                                                   Creator == other.Creator &&
                                                   Name == other.Name &&
                                                   Description == other.Description &&
                                                   Equals(Author, other.Author) &&
                                                   Equals(Copyright, other.Copyright) &&
                                                   Links.ListEquals(other.Links) &&
                                                   CreationTimeUtc == other.CreationTimeUtc &&
                                                   Keywords == other.Keywords &&
                                                   Equals(Bounds, other.Bounds) &&
                                                   Equals(Extensions, other.Extensions);

        /// <inheritdoc />
        public override int GetHashCode() => (Creator, Name, Description, Author, Copyright, Links.ListToHashCode(), CreationTimeUtc, Keywords, Bounds, Extensions).GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(Creator), Creator),
                                                                 (nameof(Name), Name),
                                                                 (nameof(Description), Description),
                                                                 (nameof(Author), Author),
                                                                 (nameof(Copyright), Copyright),
                                                                 (nameof(Links), Helpers.ListToString(Links)),
                                                                 (nameof(CreationTimeUtc), CreationTimeUtc),
                                                                 (nameof(Keywords), Keywords),
                                                                 (nameof(Bounds), Bounds),
                                                                 (nameof(Extensions), Extensions));

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Creator"/> replaced by the given value.
        /// </summary>
        /// <param name="creator">
        /// The new value for <see cref="Creator"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Creator"/> value set to <paramref name="creator"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="creator"/> is <see langword="null"/>.
        /// </exception>
        public GpxMetadata WithCreator(string creator) => new GpxMetadata(creator, Name, Description, Author, Copyright, Links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Name"/> replaced by the given value.
        /// </summary>
        /// <param name="name">
        /// The new value for <see cref="Name"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Name"/> value set to <paramref name="name"/>.
        /// </returns>
        public GpxMetadata WithName(string name) => new GpxMetadata(Creator, name, Description, Author, Copyright, Links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Description"/> replaced by the given value.
        /// </summary>
        /// <param name="description">
        /// The new value for <see cref="Description"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Description"/> value set to <paramref name="description"/>.
        /// </returns>
        public GpxMetadata WithDescription(string description) => new GpxMetadata(Creator, Name, description, Author, Copyright, Links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Author"/> replaced by the given value.
        /// </summary>
        /// <param name="author">
        /// The new value for <see cref="Author"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Author"/> value set to <paramref name="author"/>.
        /// </returns>
        public GpxMetadata WithAuthor(GpxPerson author) => new GpxMetadata(Creator, Name, Description, author, Copyright, Links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Copyright"/> replaced by the given value.
        /// </summary>
        /// <param name="copyright">
        /// The new value for <see cref="Copyright"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Copyright"/> value set to <paramref name="copyright"/>.
        /// </returns>
        public GpxMetadata WithCopyright(GpxCopyright copyright) => new GpxMetadata(Creator, Name, Description, Author, copyright, Links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Links"/> replaced by the given value.
        /// </summary>
        /// <param name="links">
        /// The new value for <see cref="Links"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Links"/> value set to <paramref name="links"/>.
        /// </returns>
        public GpxMetadata WithLinks(ImmutableArray<GpxWebLink> links) => new GpxMetadata(Creator, Name, Description, Author, Copyright, links, CreationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="CreationTimeUtc"/> replaced by the given value.
        /// </summary>
        /// <param name="creationTimeUtc">
        /// The new value for <see cref="CreationTimeUtc"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="CreationTimeUtc"/> value set to <paramref name="creationTimeUtc"/>.
        /// </returns>
        public GpxMetadata WithCreationTimeUtc(DateTime? creationTimeUtc) => new GpxMetadata(Creator, Name, Description, Author, Copyright, Links, creationTimeUtc, Keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Keywords"/> replaced by the given value.
        /// </summary>
        /// <param name="keywords">
        /// The new value for <see cref="Keywords"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Keywords"/> value set to <paramref name="keywords"/>.
        /// </returns>
        public GpxMetadata WithKeywords(string keywords) => new GpxMetadata(Creator, Name, Description, Author, Copyright, Links, CreationTimeUtc, keywords, Bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Bounds"/> replaced by the given value.
        /// </summary>
        /// <param name="bounds">
        /// The new value for <see cref="Bounds"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Bounds"/> value set to <paramref name="bounds"/>.
        /// </returns>
        public GpxMetadata WithBounds(GpxBoundingBox bounds) => new GpxMetadata(Creator, Name, Description, Author, Copyright, Links, CreationTimeUtc, Keywords, bounds, Extensions);

        /// <summary>
        /// Builds a new instance of <see cref="GpxMetadata"/> as a copy of this instance, but with
        /// <see cref="Extensions"/> replaced by the given value.
        /// </summary>
        /// <param name="extensions">
        /// The new value for <see cref="Extensions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxMetadata"/> instance that's a copy of the current instance, but
        /// with its <see cref="Extensions"/> value set to <paramref name="extensions"/>.
        /// </returns>
        public GpxMetadata WithExtensions(object extensions) => new GpxMetadata(Creator, Name, Description, Author, Copyright, Links, CreationTimeUtc, Keywords, Bounds, extensions);

        internal static GpxMetadata Load(XElement element, GpxReaderSettings settings, string creator)
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
                creationTimeUtc: Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo, settings.IgnoreBadDateTime),
                keywords: element.GpxElement("keywords")?.Value,
                bounds: GpxBoundingBox.Load(element.GpxElement("bounds")),
                extensions: extensionsElement is null ? null : settings.ExtensionReader.ConvertMetadataExtensionElement(extensionsElement.Elements()));
        }

        internal void Save(XmlWriter writer, GpxWriterSettings settings)
        {
            // caller wrote Creator (it's an attribute on the root tag)
            writer.WriteOptionalGpxElementValue("name", Name);
            writer.WriteOptionalGpxElementValue("desc", Description);
            writer.WriteOptionalGpxElementValue("author", Author);
            writer.WriteOptionalGpxElementValue("copyright", Copyright);
            writer.WriteGpxElementValues("link", Links);
            writer.WriteOptionalGpxElementValue("time", CreationTimeUtc, settings.TimeZoneInfo);
            writer.WriteOptionalGpxElementValue("keywords", Keywords);
            writer.WriteOptionalGpxElementValue("bounds", Bounds);
            writer.WriteExtensions(Extensions, settings.ExtensionWriter.ConvertMetadataExtension);
        }
    }
}
