using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents a longitude value.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_longitudeType">longitudeType</a>".
    /// </remarks>
    public readonly struct GpxLongitude : IEquatable<GpxLongitude>, IComparable<GpxLongitude>, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// The minimum legal value of <see cref="GpxLongitude"/> (-180).
        /// </summary>
        public static readonly GpxLongitude MinValue = new GpxLongitude(-180);

        /// <summary>
        /// The maximum legal value of <see cref="GpxLongitude"/> (a value very slightly smaller than 180).
        /// </summary>
        public static readonly GpxLongitude MaxValue = new GpxLongitude(Helpers.GetLargestDoubleValueSmallerThanThisPositiveFiniteValue(180));

        /// <summary>
        /// The value stored in this instance.
        /// Guaranteed to be between -180 (inclusive) and 180 (exclusive) under normal circumstances.
        /// </summary>
        /// <remarks>
        /// This value is not completely round-trip safe.  In GPX 1.1, its base type is "decimal",
        /// which supports arbitrary levels of precision.  Here, it's <see cref="double"/>, which
        /// will only faithfully store a few more than a dozen significant digits.
        /// </remarks>
        public readonly double Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxLongitude"/> struct.
        /// </summary>
        /// <param name="val">
        /// The value to store in <see cref="Value"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="val"/> is:
        /// <list type="bullet">
        /// <item><description>not a number,</description></item>
        /// <item><description>less than -180, or</description></item>
        /// <item><description>equal to 180, or</description></item>
        /// <item><description>greater than 180</description></item>
        /// </list>
        /// </exception>
        public GpxLongitude(double val)
        {
            if (!(-180 <= val && val < 180))
            {
                ThrowArgumentOutOfRangeException();
            }

            Value = val;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static bool operator ==(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value == lat2.Value;

        public static bool operator !=(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value != lat2.Value;

        public static bool operator <(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value < lat2.Value;

        public static bool operator <=(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value <= lat2.Value;

        public static bool operator >(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value >= lat2.Value;

        public static bool operator >=(GpxLongitude lat1, GpxLongitude lat2) => lat1.Value >= lat2.Value;

        public static implicit operator double(GpxLongitude lat) => lat.Value;

        public static explicit operator GpxLongitude(double val) => new GpxLongitude(val);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxLongitude other && Value == other.Value;

        /// <inheritdoc />
        public bool Equals(GpxLongitude other) => Value == other.Value;

        /// <inheritdoc />
        public int CompareTo(GpxLongitude other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (!(obj is GpxLongitude other))
            {
                ThrowArgumentException();
                return 0;
            }

            return Value.CompareTo(other.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <inheritdoc />
        public string ToString(IFormatProvider provider) => Value.ToString(provider);

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be Longitude", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between -180 and +180, inclusive");

        /// <inheritdoc />
        public TypeCode GetTypeCode() => Value.GetTypeCode();

        /// <inheritdoc />
        bool IConvertible.ToBoolean(IFormatProvider provider) => ((IConvertible)Value).ToBoolean(provider);

        /// <inheritdoc />
        byte IConvertible.ToByte(IFormatProvider provider) => ((IConvertible)Value).ToByte(provider);

        /// <inheritdoc />
        char IConvertible.ToChar(IFormatProvider provider) => ((IConvertible)Value).ToChar(provider);

        /// <inheritdoc />
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => ((IConvertible)Value).ToDateTime(provider);

        /// <inheritdoc />
        decimal IConvertible.ToDecimal(IFormatProvider provider) => ((IConvertible)Value).ToDecimal(provider);

        /// <inheritdoc />
        double IConvertible.ToDouble(IFormatProvider provider) => ((IConvertible)Value).ToDouble(provider);

        /// <inheritdoc />
        short IConvertible.ToInt16(IFormatProvider provider) => ((IConvertible)Value).ToInt16(provider);

        /// <inheritdoc />
        int IConvertible.ToInt32(IFormatProvider provider) => ((IConvertible)Value).ToInt32(provider);

        /// <inheritdoc />
        long IConvertible.ToInt64(IFormatProvider provider) => ((IConvertible)Value).ToInt64(provider);

        /// <inheritdoc />
        sbyte IConvertible.ToSByte(IFormatProvider provider) => ((IConvertible)Value).ToSByte(provider);

        /// <inheritdoc />
        float IConvertible.ToSingle(IFormatProvider provider) => ((IConvertible)Value).ToSingle(provider);

        /// <inheritdoc />
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);

        /// <inheritdoc />
        ushort IConvertible.ToUInt16(IFormatProvider provider) => ((IConvertible)Value).ToUInt16(provider);

        /// <inheritdoc />
        uint IConvertible.ToUInt32(IFormatProvider provider) => ((IConvertible)Value).ToUInt32(provider);

        /// <inheritdoc />
        ulong IConvertible.ToUInt64(IFormatProvider provider) => ((IConvertible)Value).ToUInt64(provider);
    }
}
