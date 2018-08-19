using System;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A bag of properties used to configure how <see cref="GpxReader"/> produces GPX elements.
    /// </summary>
    public sealed class GpxReaderSettings
    {
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;

        /// <summary>
        /// Gets or sets the <see cref="System.TimeZoneInfo"/> instance that the system should use
        /// to interpret timestamps in the GPX file, when the file itself does not contain time zone
        /// information.  Default is <see cref="TimeZoneInfo.Utc"/>.
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; } = UtcTimeZone;

        /// <summary>
        /// Gets or sets the <see cref="GpxExtensionReader"/> instance to use to convert GPX
        /// extension elements into (potentially) more idiomatic .NET types.  Default is an instance
        /// that preserves the original XML representations.
        /// </summary>
        public GpxExtensionReader ExtensionReader { get; set; } = new GpxExtensionReader();
    }
}
