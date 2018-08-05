using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents the ID of a DGPS station.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_dgpsStationType">dgpsStationType</a>".
    /// </remarks>
    public readonly struct GpxDgpsStationId : IEquatable<GpxDgpsStationId>, IComparable<GpxDgpsStationId>, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// The value stored in this instance.
        /// Guaranteed to be between 0 (inclusive) and 1024 (exclusive) under normal circumstances.
        /// </summary>
        public readonly ushort Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxDgpsStationId"/> struct.
        /// </summary>
        /// <param name="val">
        /// The value to store in <see cref="Value"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="val"/> is:
        /// <list type="bullet">
        /// <item><description>equal to 1024, or</description></item>
        /// <item><description>greater than 1024</description></item>
        /// </list>
        /// </exception>
        public GpxDgpsStationId(ushort val)
        {
            if (!(val < 1024))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.Value = val;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static bool operator ==(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value == deg2.Value;

        public static bool operator !=(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value != deg2.Value;

        public static bool operator <(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value < deg2.Value;

        public static bool operator <=(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value <= deg2.Value;

        public static bool operator >(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value >= deg2.Value;

        public static bool operator >=(GpxDgpsStationId deg1, GpxDgpsStationId deg2) => deg1.Value >= deg2.Value;

        public static implicit operator ushort(GpxDgpsStationId deg) => deg.Value;

        public static explicit operator GpxDgpsStationId(ushort val) => new GpxDgpsStationId(val);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxDgpsStationId other && this.Value == other.Value;

        /// <inheritdoc />
        public bool Equals(GpxDgpsStationId other) => this.Value == other.Value;

        /// <inheritdoc />
        public int CompareTo(GpxDgpsStationId other) => this.Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (!(obj is GpxDgpsStationId other))
            {
                ThrowArgumentException();
                return 0;
            }

            return this.Value.CompareTo(other.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.Value.ToString();

        /// <inheritdoc />
        public string ToString(IFormatProvider provider) => this.Value.ToString(provider);

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) => this.Value.ToString(format, formatProvider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be DgpsStationId", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between 0 (inclusive) and 1024 (exclusive)");

        /// <inheritdoc />
        public TypeCode GetTypeCode() => this.Value.GetTypeCode();

        /// <inheritdoc />
        bool IConvertible.ToBoolean(IFormatProvider provider) => ((IConvertible)this.Value).ToBoolean(provider);

        /// <inheritdoc />
        byte IConvertible.ToByte(IFormatProvider provider) => ((IConvertible)this.Value).ToByte(provider);

        /// <inheritdoc />
        char IConvertible.ToChar(IFormatProvider provider) => ((IConvertible)this.Value).ToChar(provider);

        /// <inheritdoc />
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => ((IConvertible)this.Value).ToDateTime(provider);

        /// <inheritdoc />
        decimal IConvertible.ToDecimal(IFormatProvider provider) => ((IConvertible)this.Value).ToDecimal(provider);

        /// <inheritdoc />
        double IConvertible.ToDouble(IFormatProvider provider) => ((IConvertible)this.Value).ToDouble(provider);

        /// <inheritdoc />
        short IConvertible.ToInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToInt16(provider);

        /// <inheritdoc />
        int IConvertible.ToInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToInt32(provider);

        /// <inheritdoc />
        long IConvertible.ToInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToInt64(provider);

        /// <inheritdoc />
        sbyte IConvertible.ToSByte(IFormatProvider provider) => ((IConvertible)this.Value).ToSByte(provider);

        /// <inheritdoc />
        float IConvertible.ToSingle(IFormatProvider provider) => ((IConvertible)this.Value).ToSingle(provider);

        /// <inheritdoc />
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)this.Value).ToType(conversionType, provider);

        /// <inheritdoc />
        ushort IConvertible.ToUInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt16(provider);

        /// <inheritdoc />
        uint IConvertible.ToUInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt32(provider);

        /// <inheritdoc />
        ulong IConvertible.ToUInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt64(provider);
    }
}
