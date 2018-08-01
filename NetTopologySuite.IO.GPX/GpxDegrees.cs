using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Represents an angle, in degrees.
    /// </summary>
    /// <remarks>
    /// In the official XSD schema for GPX 1.1, this corresponds to the simple type "<a href="http://www.topografix.com/GPX/1/1/#type_degreesType">degreesType</a>".
    /// </remarks>
    public readonly struct GpxDegrees : IEquatable<GpxDegrees>, IComparable<GpxDegrees>, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// The value stored in this instance.
        /// Guaranteed to be between 0 (inclusive) and 360 (exclusive) under normal circumstances.
        /// </summary>
        /// <remarks>
        /// This value is not completely round-trip safe.  In GPX 1.1, its base type is "decimal",
        /// which supports arbitrary levels of precision.  Here, it's <see cref="double"/>, which
        /// will only faithfully store a few more than a dozen significant digits.
        /// </remarks>
        public readonly double Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxDegrees"/> struct.
        /// </summary>
        /// <param name="val">
        /// The value to store in <see cref="Value"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="val"/> is:
        /// <list type="bullet">
        /// <item><description>not a number,</description></item>
        /// <item><description>less than 0,</description></item>
        /// <item><description>equal to 360, or</description></item>
        /// <item><description>greater than 360, or</description></item>
        /// </list>
        /// </exception>
        public GpxDegrees(double val)
        {
            if (!(0 <= val && val < 360))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.Value = val;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static bool operator ==(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value == deg2.Value;

        public static bool operator !=(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value != deg2.Value;

        public static bool operator <(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value < deg2.Value;

        public static bool operator <=(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value <= deg2.Value;

        public static bool operator >(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value >= deg2.Value;

        public static bool operator >=(GpxDegrees deg1, GpxDegrees deg2) => deg1.Value >= deg2.Value;

        public static implicit operator double(GpxDegrees deg) => deg.Value;

        public static explicit operator GpxDegrees(double val) => new GpxDegrees(val);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GpxDegrees other && this.Value == other.Value;

        /// <inheritdoc />
        public bool Equals(GpxDegrees other) => this.Value == other.Value;

        /// <inheritdoc />
        public int CompareTo(GpxDegrees other) => this.Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (!(obj is GpxDegrees other))
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
        public string ToString(string format, IFormatProvider formatProvider) => this.Value.ToString(format, formatProvider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be Degrees", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between 0 (inclusive) and 360 (exclusive)");

        /// <inheritdoc />
        public TypeCode GetTypeCode() => this.Value.GetTypeCode();

        /// <inheritdoc />
        public bool ToBoolean(IFormatProvider provider) => ((IConvertible)this.Value).ToBoolean(provider);

        /// <inheritdoc />
        public byte ToByte(IFormatProvider provider) => ((IConvertible)this.Value).ToByte(provider);

        /// <inheritdoc />
        public char ToChar(IFormatProvider provider) => ((IConvertible)this.Value).ToChar(provider);

        /// <inheritdoc />
        public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)this.Value).ToDateTime(provider);

        /// <inheritdoc />
        public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)this.Value).ToDecimal(provider);

        /// <inheritdoc />
        public double ToDouble(IFormatProvider provider) => ((IConvertible)this.Value).ToDouble(provider);

        /// <inheritdoc />
        public short ToInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToInt16(provider);

        /// <inheritdoc />
        public int ToInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToInt32(provider);

        /// <inheritdoc />
        public long ToInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToInt64(provider);

        /// <inheritdoc />
        public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)this.Value).ToSByte(provider);

        /// <inheritdoc />
        public float ToSingle(IFormatProvider provider) => ((IConvertible)this.Value).ToSingle(provider);

        /// <inheritdoc />
        public string ToString(IFormatProvider provider) => this.Value.ToString(provider);

        /// <inheritdoc />
        public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)this.Value).ToType(conversionType, provider);

        /// <inheritdoc />
        public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt16(provider);

        /// <inheritdoc />
        public uint ToUInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt32(provider);

        /// <inheritdoc />
        public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt64(provider);
    }
}
