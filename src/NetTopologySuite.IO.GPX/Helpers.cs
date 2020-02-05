using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    internal static class Helpers
    {
        public const string GpxNamespace = "http://www.topografix.com/GPX/1/1";

        private static readonly int RandomSeed = new Random().Next(int.MinValue, int.MaxValue);

        private static readonly string[] FixKindStrings = { "none", "2d", "3d", "dgps", "pps" };

        private static readonly Regex YearParseRegex = new Regex(@"^(?<yearFrag>-?(([1-9]\d\d\d+)|(0\d\d\d)))(Z|([+-]((((0\d)|(1[0-3])):[0-5]\d)|(14:00))))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly string MaxPrecisionFormatString = "0." + new string('#', 324);

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value) => (key, value) = (kvp.Key, kvp.Value);

        // dotnet/corefx#22625
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ImmutableArray<T> array) => Unsafe.As<ImmutableArray<T>, T[]>(ref array);

        public static bool TryGetCount<T>(this IEnumerable<T> source, out int count)
        {
            switch (source)
            {
                case ICollection<T> icollectionOfT:
                    count = icollectionOfT.Count;
                    return true;

                case System.Collections.ICollection icollection:
                    count = icollection.Count;
                    return true;

                case IReadOnlyCollection<T> ireadOnlyCollectionOfT:
                    count = ireadOnlyCollectionOfT.Count;
                    return true;

                default:
                    count = 0;
                    return false;
            }
        }

        // https://github.com/dotnet/coreclr/blob/cc52c67f5a0a26194c42fbd1b59e284d6727635a/src/System.Private.CoreLib/shared/System/Double.cs#L47-L54
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(this double d)
        {
            long bits = BitConverter.DoubleToInt64Bits(d);
            return (bits & 0x7FFFFFFFFFFFFFFF) < 0x7FF0000000000000;
        }

        // https://github.com/dotnet/coreclr/blob/51c3dc3bbd5e7515cbc03249c5e34b239a87b281/src/System.Private.CoreLib/src/System/Numerics/Hashing/HashHelpers.cs#L17-L18
        public static int HashHelpersCombine(int h1, int h2)
        {
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }

        public static string ToRoundTripString(this double val, IFormatProvider formatProvider)
        {
            Debug.Assert(val.IsFinite(), "This should only be used with finite values.");
            string result = val.ToString("R", formatProvider);
            if (!(TryParseDouble(result, out double attemptedRoundTrip) && val == attemptedRoundTrip))
            {
                // "R" uses exponential notation past a certain point, which we (and the GPX spec)
                // won't be able to parse back later.  Use a big ol' format string instead.
                result = val.ToString(MaxPrecisionFormatString, formatProvider);
            }

            // technically, this isn't perfect: a few numbers get in here that don't round-trip
            // exactly (0.000063416082441534885 and 0.000073552131687082412 show up), but I'm a bit
            // sick of trying to deal with them, so I'm going to call it good enough.
            return result;
        }

        public static double GetLargestDoubleValueSmallerThanThisPositiveFiniteValue(double maxExclusive)
        {
            long bits = BitConverter.DoubleToInt64Bits(maxExclusive);
            --bits;
            return BitConverter.Int64BitsToDouble(bits);
        }

        public static string ListToString<TElement>(ImmutableArray<TElement> lst)
        {
            if (lst.IsDefaultOrEmpty)
            {
                return null;
            }

            var sb = new StringBuilder("[");
            int i = 0;
            while (true)
            {
                sb.Append(lst[i]);
                if (++i == lst.Length)
                {
                    return sb.Append("]").ToString();
                }
                else
                {
                    sb.Append(", ");
                }
            }
        }

        public static int ListToHashCode<TElement>(this ImmutableArray<TElement> lst, IEqualityComparer<TElement> comparer = null)
        {
            if (lst.IsDefaultOrEmpty)
            {
                return 0;
            }

            if (default(TElement) == null)
            {
                // https://github.com/dotnet/coreclr/issues/17273
                comparer = comparer ?? EqualityComparer<TElement>.Default;
            }

            if (comparer is null)
            {
                int hc = RandomSeed;
                for (int i = 0; i < lst.Length; i++)
                {
                    hc = HashHelpersCombine(hc, EqualityComparer<TElement>.Default.GetHashCode(lst[i]));
                }

                return hc;
            }
            else
            {
                int hc = RandomSeed;
                for (int i = 0; i < lst.Length; i++)
                {
                    hc = HashHelpersCombine(hc, comparer.GetHashCode(lst[i]));
                }

                return hc;
            }
        }

        public static int ListToHashCode<TElement>(this ImmutableArray<TElement> lst)
            where TElement : unmanaged
        {
            var bytes = MemoryMarshal.AsBytes(lst.AsReadOnlySpan());
            return xxHash64.Hash(bytes).GetHashCode();
        }

        public static bool ListEquals<TElement>(this ImmutableArray<TElement> lst1, ImmutableArray<TElement> lst2, IEqualityComparer<TElement> comparer = null)
        {
            if (lst1.IsDefault)
            {
                return lst2.IsDefault;
            }

            if (lst2.IsDefault || lst1.Length != lst2.Length)
            {
                return false;
            }

            if (default(TElement) == null)
            {
                // https://github.com/dotnet/coreclr/issues/17273
                comparer = comparer ?? EqualityComparer<TElement>.Default;
            }

            if (comparer is null)
            {
                for (int i = 0; i < lst1.Length; i++)
                {
                    if (!EqualityComparer<TElement>.Default.Equals(lst1[i], lst2[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                for (int i = 0; i < lst1.Length; i++)
                {
                    if (!comparer.Equals(lst1[i], lst2[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static bool ListEquals<TElement>(this ImmutableArray<TElement> lst1, ImmutableArray<TElement> lst2)
            where TElement : struct, IEquatable<TElement>
        {
            // this works just fine for default / empty on either side.
            return lst1.AsReadOnlySpan().SequenceEqual(lst2.AsReadOnlySpan());
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

        public static UriValidationResult InterpretUri(string text, out Uri bestEffortUri)
        {
            if (text is null)
            {
                bestEffortUri = null;
                return UriValidationResult.NullValue;
            }

            if (Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out bestEffortUri))
            {
                return UriValidationResult.ValidSystemUri;
            }

            // dotnet/runtime#1857: System.Uri doesn't support URI values with length >= 65520, but
            // at least one consumer of this library has a legitimate use case for data URIs that
            // might hit that limit.  this method is here to help support that a bit.
            if (text.Length < 65520)
            {
                // as far as I know, the framework behaves correctly for URI values that are *below*
                // the limit (at least, no issues have been reported so far), so let's trust it.
                return UriValidationResult.InvalidUri;
            }

            // I'm not exactly interested in implementing a general-purpose URI validation toolkit,
            // so this is only a best-effort attempt (e.g., it ignores invalid base64).
            return Regex.IsMatch(text, @"^\s*data:([+\-\w\.]+\/[+\-\w\.]+(;[+\-\w\.]+=[+\-\w\.]+)*)?(;base64)?,.*?\s*$", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture)
                ? UriValidationResult.ValidOverlongDataUri
                : UriValidationResult.InvalidUri;
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

        public static void WriteOptionalGpxElementValue(this XmlWriter writer, string localName, DateTime? valueUtc, TimeZoneInfo timeZoneInfo)
        {
            if (valueUtc.HasValue)
            {
                // Format string is hardcoded, instead of allowing the user to override it, to
                // ensure that we always write out timestamps that are valid according to the
                // published XML schema.
                const string Format = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

                string text;
                if (timeZoneInfo == TimeZoneInfo.Utc)
                {
                    text = valueUtc.GetValueOrDefault().ToString(Format, CultureInfo.InvariantCulture);
                }
                else
                {
                    // only use this path when we truly need to convert.  not only is the other path
                    // faster (probably), but DateTimeOffset's implementation always seems to write
                    // "+00:00" for UTC, instead of just "Z".
                    var value = new DateTimeOffset(valueUtc.GetValueOrDefault());
                    value = TimeZoneInfo.ConvertTime(value, timeZoneInfo);
                    text = value.ToString(Format, CultureInfo.InvariantCulture);
                }

                writer.WriteGpxElementString(localName, text);
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

        private static bool TryParseDouble(string text, out double result) => double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

        public static double? ParseDouble(string text)
        {
            if (text is null)
            {
                return null;
            }

            return TryParseDouble(text, out double result) && result.IsFinite()
                ? result
                : throw new XmlException("decimal must be formatted properly");
        }

        public static GpxLongitude? ParseLongitude(string text)
        {
            if (text is null)
            {
                return null;
            }

            return TryParseDouble(text, out double result) && -180 <= result && result < 180
                ? new GpxLongitude(result)
                : throw new XmlException("longitude must be formatted properly and be between -180 (inclusive) and +180 (exclusive)");
        }

        public static GpxLatitude? ParseLatitude(string text)
        {
            if (text is null)
            {
                return null;
            }

            return TryParseDouble(text, out double result) && Math.Abs(result) <= 90
                ? new GpxLatitude(result)
                : throw new XmlException("latitude must be formatted properly and be between -90 (inclusive) and +90 (inclusive)");
        }

        public static GpxDegrees? ParseDegrees(string text)
        {
            if (text is null)
            {
                return null;
            }

            return TryParseDouble(text, out double result) && 0 <= result && result < 360
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
                : throw new XmlException("DGPS station ID must be formatted properly and be between 0 (inclusive) and 1024 (exclusive)");
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

        public static DateTime? ParseDateTimeUtc(string text, TimeZoneInfo timeZoneInfo, bool ignoreParseFailures)
        {
            if (text is null)
            {
                return null;
            }

            if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal, out var result))
            {
                return ignoreParseFailures
                    ? default(DateTime?)
                    : throw new XmlException("time element must be formatted properly");
            }

            if (result.Kind == DateTimeKind.Unspecified)
            {
                result = TimeZoneInfo.ConvertTime(result, timeZoneInfo, TimeZoneInfo.Utc);
            }

            return result;
        }
    }
}
