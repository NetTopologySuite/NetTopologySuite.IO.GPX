using System;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A bag of properties used to configure how <see cref="GpxWriter"/> produces XML elements that
    /// correspond to the original GPX data objects.
    /// </summary>
    public sealed class GpxWriterSettings
    {
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;

        private static readonly GpxExtensionWriter DefaultExtensionWriter = new GpxExtensionWriter();

        /// <summary>
        /// Gets or sets the <see cref="System.TimeZoneInfo"/> instance that the system should use
        /// to produce timestamps for the GPX file.  Default is <see cref="TimeZoneInfo.Utc"/>.
        /// When overwriting this property, note that the XSD schema specifies that, by convention,
        /// timestamps in GPX files are expected to be in UTC.
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; } = UtcTimeZone;

        /// <summary>
        /// Gets or sets the <see cref="GpxExtensionWriter"/> instance to use to convert
        /// (potentially) idiomatic .NET types into the corresponding XML representation.  Default
        /// is an instance of the base class (see its summary documentation for details).
        /// </summary>
        public GpxExtensionWriter ExtensionWriter { get; set; } = DefaultExtensionWriter;
    }
}
