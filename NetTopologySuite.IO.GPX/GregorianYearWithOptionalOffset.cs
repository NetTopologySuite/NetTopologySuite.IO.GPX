using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NetTopologySuite.IO
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct GregorianYearWithOptionalOffset
    {
        private static readonly Regex ParseRegex = new Regex(@"^(?<yearFrag>-?(([1-9]\d\d\d+)|(0\d\d\d)))(?<timezoneFrag>(Z|[+-])((((0\d)|(1[0-3])):[0-5]\d)|(14:00)))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly int Year;

        public readonly short? OffsetMinutes;

        public GregorianYearWithOptionalOffset(int year, short? offsetMinutes)
        {
            if (offsetMinutes < -840 || offsetMinutes > 840)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetMinutes), offsetMinutes, "Must be between -840 and 840, inclusive, or null");
            }

            this.Year = year;
            this.OffsetMinutes = offsetMinutes;
        }

        public static bool TryParse(string text, out GregorianYearWithOptionalOffset result)
        {
            result = default;
            if (text is null)
            {
                return false;
            }

            int start;
            for (start = 0; start < text.Length && char.IsWhiteSpace(text, start); start++)
            {
            }

            int end;
            for (end = text.Length - 1; end > start && char.IsWhiteSpace(text, end); end--)
            {
            }

            var match = ParseRegex.Match(text, start, end - start + 1);
            if (!match.Success)
            {
                return false;
            }

            var yearFrag = match.Groups["yearFrag"];
            int year = int.Parse(yearFrag.Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);

            var timezoneFrag = match.Groups["timezoneFrag"];
            short? offsetMinutes = null;
            if (timezoneFrag.Success)
            {
                string timezoneFragValue = timezoneFrag.Value;
                int minutes = (       timezoneFragValue[timezoneFragValue.Length - 1] - '0') +
                              (10 *  (timezoneFragValue[timezoneFragValue.Length - 2] - '0')) +
                              (60 *  (timezoneFragValue[timezoneFragValue.Length - 4] - '0')) +
                              (600 * (timezoneFragValue[timezoneFragValue.Length - 5] - '0'));
                if (timezoneFragValue[0] == '-')
                {
                    minutes = -minutes;
                }

                offsetMinutes = (short)minutes;
            }

            result = new GregorianYearWithOptionalOffset(year, offsetMinutes);
            return true;
        }
    }
}
