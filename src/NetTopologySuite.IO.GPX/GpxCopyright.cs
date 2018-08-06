﻿using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents copyright-related information.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the complex type "<a href="http://www.topografix.com/GPX/1/1/#type_copyrightType">copyrightType</a>".
    /// </remarks>
    public sealed class GpxCopyright : ICanWriteToXmlWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxCopyright"/> class.
        /// </summary>
        /// <param name="author">
        /// The value for <see cref="Author"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="author"/> is <see langword="null" />.
        /// </exception>
        public GpxCopyright(string author)
            : this(author, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxCopyright"/> class.
        /// </summary>
        /// <param name="author">
        /// The value for <see cref="Author"/>.
        /// </param>
        /// <param name="year">
        /// The value for <see cref="Year"/>.
        /// </param>
        /// <param name="licenseUri">
        /// The value for <see cref="LicenseUri"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="author"/> is <see langword="null" />.
        /// </exception>
        public GpxCopyright(string author, int? year, Uri licenseUri)
        {
            this.Author = author ?? throw new ArgumentNullException(nameof(author));
            this.Year = year;
            this.LicenseUri = licenseUri;
        }

        /// <summary>
        /// Gets the author who holds the copyright.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "author" attribute.
        /// </remarks>
        public string Author { get; }

        /// <summary>
        /// Gets an optional Gregorian year for the copyright.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "year" element.
        /// </para>
        /// <para>
        /// This value is not completely round-trip safe.  In GPX 1.1, its type is "gYear", which
        /// would technically support values less than <see cref="int.MinValue"/> and greater than
        /// <see cref="int.MaxValue"/>.  Less pedantically, "gYear" also supports an optional offset
        /// from UTC.  When loading from XML, a time zone specifier (if present) is observed,
        /// validated, and then ignored.
        /// </para>
        /// </remarks>
        public int? Year { get; }

        /// <summary>
        /// Gets an optional <see cref="Uri"/> that identifies the copyright license.
        /// </summary>
        /// <remarks>
        /// In the official XSD schema for GPX 1.1, this corresponds to the "license" element.
        /// </remarks>
        public Uri LicenseUri { get; }

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Author), this.Author),
                                                                 (nameof(this.Year), this.Year),
                                                                 (nameof(this.LicenseUri), this.LicenseUri));

        /// <summary>
        /// Builds a new instance of <see cref="GpxCopyright"/> as a copy of this instance, but with
        /// <see cref="Year"/> replaced by the given value.
        /// </summary>
        /// <param name="year">
        /// The new value for <see cref="Year"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxCopyright"/> instance that's a copy of the current instance, but
        /// with its <see cref="Year"/> value set to <paramref name="year"/>.
        /// </returns>
        public GpxCopyright WithYear(int? year) => new GpxCopyright(this.Author, year, this.LicenseUri);

        /// <summary>
        /// Builds a new instance of <see cref="GpxCopyright"/> as a copy of this instance, but with
        /// <see cref="LicenseUri"/> replaced by the given value.
        /// </summary>
        /// <param name="licenseUri">
        /// The new value for <see cref="LicenseUri"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="GpxCopyright"/> instance that's a copy of the current instance, but
        /// with its <see cref="LicenseUri"/> value set to <paramref name="licenseUri"/>.
        /// </returns>
        public GpxCopyright WithLicenseUri(Uri licenseUri) => new GpxCopyright(this.Author, this.Year, licenseUri);

        internal static GpxCopyright Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxCopyright(
                author: element.Attribute("author")?.Value ?? throw new XmlException("copyright element must have author attribute."),
                year: Helpers.ParseGregorianYear(element.GpxElement("year")?.Value),
                licenseUri: Helpers.ParseUri(element.GpxElement("license")?.Value));
        }

        void ICanWriteToXmlWriter.Save(XmlWriter writer)
        {
            writer.WriteAttributeString("author", this.Author);
            writer.WriteOptionalGpxElementValue("year", this.Year?.ToString("0000", CultureInfo.InvariantCulture));
            writer.WriteOptionalGpxElementValue("license", this.LicenseUri?.OriginalString);
        }
    }
}