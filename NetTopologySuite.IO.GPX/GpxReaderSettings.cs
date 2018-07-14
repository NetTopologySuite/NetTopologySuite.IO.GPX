using System;

namespace NetTopologySuite.IO
{
    public sealed class GpxReaderSettings
    {
        private static readonly TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

        public TimeZoneInfo TimeZoneInfo { get; set; } = LocalTimeZone;
    }
}
