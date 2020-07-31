using System;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A bag of properties used to configure how <see cref="GpxReader"/> produces GPX data objects
    /// that correspond to the original XML elements.
    /// </summary>
    public sealed class GpxReaderSettings
    {
        private static readonly GpxExtensionReader DefaultExtensionReader = new GpxExtensionReader();

        private TimeZoneInfo _timeZoneInfo;

        /// <summary>
        /// Gets or sets the <see cref="System.TimeZoneInfo"/> instance that the system should use
        /// to interpret timestamps in the GPX file, when the file itself does not contain time zone
        /// information.  Default is <see cref="TimeZoneInfo.Utc"/>.
        /// <para>
        /// <see langword="null"/> is treated as <see cref="TimeZoneInfo.Utc"/>, but please prefer
        /// <see langword="null"/> so that <see cref="TimeZoneInfo.ClearCachedData"/> does not
        /// affect our correctness.
        /// </para>
        /// </summary>
        public TimeZoneInfo TimeZoneInfo
        {
            get => _timeZoneInfo ?? TimeZoneInfo.Utc;
            set => _timeZoneInfo = value;
        }

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

        /// <summary>
        /// Gets or sets a value indicating whether or not to build <see cref="GpxWebLink"/> objects
        /// for strings whose lengths are greater than or equal to 65520 characters.  Older versions
        /// of this library would throw exceptions for such strings (see dotnet/runtime#1857 and
        /// NetTopologySuite/NetTopologySuite.IO.GPX#39), and the workaround involves relaxing an
        /// invariant of <see cref="GpxWebLink"/>, so it is guarded by an opt-in flag to avoid a
        /// breaking change.
        /// </summary>
        public bool BuildWebLinksForVeryLongUriValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to ignore files with elements not part of the contract
        /// even though such files would not pass XSD validation (see
        /// NetTopologySuite/NetTopologySuite.IO.GPX#29).
        /// </summary>
        public bool IgnoreBadElements { get; set; }
    }
}
