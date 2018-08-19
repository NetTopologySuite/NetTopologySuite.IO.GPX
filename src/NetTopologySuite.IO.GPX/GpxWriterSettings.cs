using System;

namespace NetTopologySuite.IO
{
    public sealed class GpxWriterSettings
    {
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;

        public TimeZoneInfo TimeZoneInfo { get; set; } = UtcTimeZone;

        public GpxExtensionWriter ExtensionWriter { get; set; } = new GpxExtensionWriter();
    }
}
