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

        /// <summary>
        /// Gets or sets the value to fill in for <see cref="GpxMetadata.Creator"/> if a value was
        /// not given.  Provided to help work around a bug from an old version of Israel Hiking Map
        /// (see NetTopologySuite/NetTopologySuite.IO.GPX#23).
        /// </summary>
        public string DefaultCreatorIfMissing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to ignore files that do not contain
        /// <c>version="1.1"</c> exactly, even though such files would not pass XSD validation (see
        /// NetTopologySuite/NetTopologySuite.IO.GPX#27 and #28).
        /// </summary>
        public bool IgnoreVersionAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to ignore files with timestamps that do
        /// not parse, even though such files would not pass XSD validation (see
        /// NetTopologySuite/NetTopologySuite.IO.GPX#29).
        /// </summary>
        public bool IgnoreBadDateTime { get; set; }
    }
}
