using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    internal static class Helpers
    {
        public const string GpxNamespace = "http://www.topografix.com/GPX/1/1";

        public static string ListToString<TElement>(IEnumerable<TElement> lst)
        {
            var sb = new StringBuilder("[");
            bool appended = false;
            foreach (var value in lst)
            {
                if (appended)
                {
                    sb.Append(", ");
                }
                else
                {
                    appended = true;
                }

                sb.Append(value);
            }

            return sb.Append("]").ToString();
        }

        public static string BuildString(params (string fieldName, object fieldValue)[] values)
        {
            var sb = new StringBuilder("[");
            foreach ((string fieldName, object fieldValue) in values)
            {
                if (fieldValue is null)
                {
                    continue;
                }

                if (sb.Length > 1)
                {
                    sb.Append(", ");
                }

                sb.Append(fieldName);
                sb.Append(": ");
                sb.Append(fieldValue);
            }

            return sb.Append("]").ToString();
        }

        public static XAttribute GpxAttribute(this XElement element, string localName) => element.Attribute(localName);

        public static XElement GpxElement(this XElement element, string localName) => element.Element(XName.Get(localName, GpxNamespace));

        public static IEnumerable<XElement> GpxElements(this XElement element, string localName) => element.Elements(XName.Get(localName, GpxNamespace));

        public static uint? ParseUInt32(string text)
        {
            if (text is null)
            {
                return null;
            }

            return uint.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out uint result)
                ? result
                : throw new XmlException("nonNegativeInteger must be formatted properly");
        }

        public static double? ParseDouble(string text)
        {
            if (text is null)
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result)
                ? result
                : throw new XmlException("decimal must be formatted properly");
        }

        public static GpxLongitude? ParseLongitude(string text)
        {
            if (text is null)
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result) && Math.Abs(result) <= 180
                ? new GpxLongitude(result)
                : throw new XmlException("longitude must be formatted properly and be between -180 and +180 inclusive");
        }

        public static GpxLatitude? ParseLatitude(string text)
        {
            if (text is null)
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result) && Math.Abs(result) <= 90
                ? new GpxLatitude(result)
                : throw new XmlException("latitude must be formatted properly and be between -90 and +90 inclusive");
        }

        public static GpxDegrees? ParseDegrees(string text)
        {
            if (text is null)
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result) && 0 <= result && result < 360
                ? new GpxDegrees(result)
                : throw new XmlException("degrees must be formatted properly and be between 0 (inclusive) and 360 (exclusive)");
        }

        public static GpxDgpsStationId? ParseDgpsStationId(string text)
        {
            if (text is null)
            {
                return null;
            }

            return ushort.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out ushort result) && 0 <= result && result < 1024
                ? new GpxDgpsStationId(result)
                : throw new XmlException("DGPS station ID must be formatted properly and be between 0 and 1023 inclusive");
        }

        public static GpxFixKind? ParseFixKind(string text)
        {
            switch (text)
            {
                case null:
                    return null;

                case "none":
                    return GpxFixKind.None;

                case "2d":
                    return GpxFixKind.TwoDimensional;

                case "3d":
                    return GpxFixKind.ThreeDimensional;

                case "dgps":
                    return GpxFixKind.DGPS;

                case "pps":
                    return GpxFixKind.PPS;

                default:
                    throw new XmlException("fix must be either 'none', '2d', '3d', 'dgps', or 'pps'");
            }
        }

        public static Uri ParseUri(string text)
        {
            if (text is null)
            {
                return null;
            }

            return Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var result)
                ? result
                : throw new XmlException("uri must be formatted properly");
        }

        public static GregorianYearWithOptionalOffset? ParseGregorianYearWithOptionalOffset(string text)
        {
            if (text is null)
            {
                return null;
            }

            return GregorianYearWithOptionalOffset.TryParse(text, out var result)
                ? result
                : throw new XmlException("year element must be formatted properly");
        }

        public static DateTime? ParseDateTimeUtc(string text, TimeZoneInfo timeZoneInfo)
        {
            if (text is null)
            {
                return null;
            }

            if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal, out var result))
            {
                throw new XmlException("time element must be formatted properly");
            }

            if (result.Kind == DateTimeKind.Unspecified)
            {
                result = TimeZoneInfo.ConvertTime(result, timeZoneInfo, TimeZoneInfo.Utc);
            }

            return result;
        }
    }
}
