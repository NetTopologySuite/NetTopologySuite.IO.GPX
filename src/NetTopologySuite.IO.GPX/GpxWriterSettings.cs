using System;
using System.Collections.Generic;
using System.Xml;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A bag of properties used to configure how <see cref="GpxWriter"/> produces XML elements that
    /// correspond to the original GPX data objects.
    /// </summary>
    public sealed class GpxWriterSettings
    {
        private static readonly GpxExtensionWriter DefaultExtensionWriter = new GpxExtensionWriter();

        private TimeZoneInfo timeZoneInfo;

        /// <summary>
        /// Gets or sets the <see cref="System.TimeZoneInfo"/> instance that the system should use
        /// to produce timestamps for the GPX file.  Default is <see cref="TimeZoneInfo.Utc"/>.
        /// When overwriting this property, note that the XSD schema specifies that, by convention,
        /// timestamps in GPX files are expected to be in UTC.
        /// <para>
        /// <see langword="null"/> is treated as <see cref="TimeZoneInfo.Utc"/>, but please prefer
        /// <see langword="null"/> so that <see cref="TimeZoneInfo.ClearCachedData"/> does not
        /// affect our correctness.
        /// </para>
        /// </summary>
        public TimeZoneInfo TimeZoneInfo
        {
            get => timeZoneInfo ?? TimeZoneInfo.Utc;
            set => timeZoneInfo = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="GpxExtensionWriter"/> instance to use to convert
        /// (potentially) idiomatic .NET types into the corresponding XML representation.  Default
        /// is an instance of the base class (see its summary documentation for details).
        /// </summary>
        public GpxExtensionWriter ExtensionWriter { get; set; } = DefaultExtensionWriter;

        /// <summary>
        /// Gets an <see cref="IDictionary{TKey, TValue}"/> instance that can be used to register
        /// additional namespaces to declare on the root element, keyed by the desired prefix to use
        /// in the result.  These can be used to avoid duplicating the namespace declaration for
        /// every single "extensions" element that uses the exact same namespace.
        /// <para>
        /// The <see cref="XmlWriterSettings"/>'s <see cref="XmlWriterSettings.NamespaceHandling"/>
        /// value will need to be set to <see cref="NamespaceHandling.OmitDuplicates"/> when the
        /// writer is created in order to fully benefit from using this property.
        /// </para>
        /// </summary>
        public IDictionary<string, Uri> CommonXmlNamespacesByDesiredPrefix { get; } = new Dictionary<string, Uri>();
    }
}
