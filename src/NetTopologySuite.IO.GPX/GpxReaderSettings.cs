using System;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A bag of properties used to configure how <see cref="GpxReader"/> produces GPX data objects
    /// that correspond to the original XML elements.
    /// </summary>
    public sealed class GpxReaderSettings
    {
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;

        private static readonly GpxExtensionReader DefaultExtensionReader = new GpxExtensionReader();

        /// <summary>
        /// Gets or sets the <see cref="System.TimeZoneInfo"/> instance that the system should use
        /// to interpret timestamps in the GPX file, when the file itself does not contain time zone
        /// information.  Default is <see cref="TimeZoneInfo.Utc"/>.
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; } = UtcTimeZone;

        /// <summary>
        /// Gets or sets the <see cref="GpxExtensionReader"/> instance to use to convert GPX
        /// extension elements into (potentially) more idiomatic .NET types.  Default is an instance
        /// of the base class (see its summary documentation for details).
        /// </summary>
        public GpxExtensionReader ExtensionReader { get; set; } = DefaultExtensionReader;
    }
}
