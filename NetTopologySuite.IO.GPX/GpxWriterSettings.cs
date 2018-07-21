using System;

namespace NetTopologySuite.IO
{
    public sealed class GpxWriterSettings
    {
        private static readonly TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

        public TimeZoneInfo TimeZoneInfo { get; set; } = LocalTimeZone;

        public GpxExtensionWriter ExtensionWriter { get; set; } = new GpxExtensionWriter();
    }
}
