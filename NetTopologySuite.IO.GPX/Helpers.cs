using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    internal static class Helpers
    {
        public const string GpxNamespace = "http://www.topografix.com/GPX/1/1";

        private static readonly string[] FixKindStrings = { "none", "2d", "3d", "dgps", "pps" };

        private static readonly Regex YearParseRegex = new Regex(@"^(?<yearFrag>-?(([1-9]\d\d\d+)|(0\d\d\d)))(Z|([+-]((((0\d)|(1[0-3])):[0-5]\d)|(14:00))))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // https://github.com/dotnet/coreclr/blob/cc52c67f5a0a26194c42fbd1b59e284d6727635a/src/System.Private.CoreLib/shared/System/Double.cs#L47-L54
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(this double d)
        {
            long bits = BitConverter.DoubleToInt64Bits(d);
            return (bits & 0x7FFFFFFFFFFFFFFF) < 0x7FF0000000000000;
        }

        public static string ToRoundTripString(this double val, IFormatProvider formatProvider)
        {
            string result = val.ToString("R", formatProvider);

            // work around dotnet/coreclr#13106
            if (val.IsFinite())
            {
                if (val != double.Parse(result, formatProvider))
                {
                    result = val.ToString("G16", formatProvider);
                    if (val != double.Parse(result, formatProvider))
                    {
                        result = val.ToString("G17", formatProvider);
                    }
                }
            }

            return result;
        }

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

        public static XElement GpxElement(this XElement element, string localName) => element.Element(XName.Get(localName, GpxNamespace));

        public static IEnumerable<XElement> GpxElements(this XElement element, string localName) => element.Elements(XName.Get(localName, GpxNamespace));

        public static void WriteGpxElementString(this XmlWriter writer, string localName, string value) => writer.WriteElementString(localName, GpxNamespace, value);

        public static void WriteGpxStartElement(this XmlWriter writer, string localName) => writer.WriteStartElement(localName, GpxNamespace);

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, string value)
        {
            if (!(value is null))
            {
                writer.WriteGpxElementString(localName, value);
            }
        }

        public static void WriteExtensions(this XmlWriter writer, object extensions, Func<object, IEnumerable<XElement>> extensionCallback)
        {
            IEnumerable<XElement> elements;
            if (extensions is null || (elements = extensionCallback(extensions)) is null)
            {
                return;
            }

            writer.WriteGpxStartElement("extensions");
            foreach (var element in elements)
            {
                element.WriteTo(writer);
            }

            writer.WriteEndElement();
        }

        public static void WriteOptionalGpxElementValue<T>(this XmlWriter writer, string localName, T value)
            where T : class, ICanWriteToXmlWriter
        {
            if (!(value is null))
            {
                writer.WriteGpxStartElement(localName);
                value.Save(writer);
                writer.WriteEndElement();
            }
        }

        public static void WriteGpxElementValues<T>(this XmlWriter writer, string localName, ImmutableArray<T> values)
            where T : ICanWriteToXmlWriter
        {
            foreach (var value in values)
            {
                writer.WriteGpxStartElement(localName);
                value.Save(writer);
                writer.WriteEndElement();
            }
        }

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, GpxFixKind? value)
        {
            if (value.HasValue)
            {
                int intVal = (int)value.GetValueOrDefault();
                if (!unchecked((uint)intVal < (uint)FixKindStrings.Length))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Unrecognized GpxFixKind value");
                }

                writer.WriteGpxElementString(localName, FixKindStrings[intVal]);
            }
        }

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, uint? value)
        {
            if (value.HasValue)
            {
                writer.WriteGpxElementString(localName, value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture));
            }
        }

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, double? value)
        {
            if (value.HasValue)
            {
                writer.WriteGpxElementString(localName, value.GetValueOrDefault().ToRoundTripString(CultureInfo.InvariantCulture));
            }
        }

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, DateTime? valueUtc)
        {
            if (valueUtc.HasValue)
            {
                writer.WriteGpxElementString(localName, valueUtc.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
            }
        }

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
            if (text is null)
            {
                return null;
            }

            int idx = Array.IndexOf(FixKindStrings, text);
            if (idx < 0)
            {
                throw new XmlException("fix must be either 'none', '2d', '3d', 'dgps', or 'pps'");
            }

            return (GpxFixKind)idx;
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

        public static int? ParseGregorianYear(string text)
        {
            if (text is null)
            {
                return null;
            }

            int start;
            for (start = 0; start < text.Length && char.IsWhiteSpace(text, start); start++)
            {
            }

            int end;
            for (end = text.Length - 1; end > start && char.IsWhiteSpace(text, end); end--)
            {
            }

            var match = YearParseRegex.Match(text, start, end - start + 1);
            if (!match.Success)
            {
                throw new XmlException("year element must be formatted properly");
            }

            var yearFrag = match.Groups["yearFrag"];
            return int.TryParse(yearFrag.Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int result)
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
