using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    public readonly struct GpxDegrees : IEquatable<GpxDegrees>, IComparable<GpxDegrees>, IComparable, IFormattable, IConvertible
    {
        private readonly double value;

        public GpxDegrees(double val)
        {
            if (!(0 <= val && val < 360))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.value = val;
        }

        public static bool operator ==(GpxDegrees deg1, GpxDegrees deg2) => deg1.value == deg2.value;

        public static bool operator !=(GpxDegrees deg1, GpxDegrees deg2) => deg1.value != deg2.value;

        public static bool operator <(GpxDegrees deg1, GpxDegrees deg2) => deg1.value < deg2.value;

        public static bool operator <=(GpxDegrees deg1, GpxDegrees deg2) => deg1.value <= deg2.value;

        public static bool operator >(GpxDegrees deg1, GpxDegrees deg2) => deg1.value >= deg2.value;

        public static bool operator >=(GpxDegrees deg1, GpxDegrees deg2) => deg1.value >= deg2.value;

        public static implicit operator double(GpxDegrees deg) => deg.value;

        public static explicit operator GpxDegrees(double val) => new GpxDegrees(val);

        public override bool Equals(object obj) => obj is GpxDegrees other && this.value == other.value;

        public bool Equals(GpxDegrees other) => this.value == other.value;

        public int CompareTo(GpxDegrees other) => this.value.CompareTo(other.value);

        public int CompareTo(object obj)
        {
            if (!(obj is GpxDegrees other))
            {
                ThrowArgumentException();
                return 0;
            }

            return this.value.CompareTo(other.value);
        }

        public override int GetHashCode() => this.value.GetHashCode();

        public override string ToString() => this.value.ToString();

        public string ToString(string format, IFormatProvider formatProvider) => this.value.ToString(format, formatProvider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be Degrees", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between 0 (inclusive) and 360 (exclusive)");

        public TypeCode GetTypeCode() => this.value.GetTypeCode();
        public bool ToBoolean(IFormatProvider provider) => ((IConvertible)this.value).ToBoolean(provider);
        public byte ToByte(IFormatProvider provider) => ((IConvertible)this.value).ToByte(provider);
        public char ToChar(IFormatProvider provider) => ((IConvertible)this.value).ToChar(provider);
        public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)this.value).ToDateTime(provider);
        public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)this.value).ToDecimal(provider);
        public double ToDouble(IFormatProvider provider) => ((IConvertible)this.value).ToDouble(provider);
        public short ToInt16(IFormatProvider provider) => ((IConvertible)this.value).ToInt16(provider);
        public int ToInt32(IFormatProvider provider) => ((IConvertible)this.value).ToInt32(provider);
        public long ToInt64(IFormatProvider provider) => ((IConvertible)this.value).ToInt64(provider);
        public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)this.value).ToSByte(provider);
        public float ToSingle(IFormatProvider provider) => ((IConvertible)this.value).ToSingle(provider);
        public string ToString(IFormatProvider provider) => this.value.ToString(provider);
        public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)this.value).ToType(conversionType, provider);
        public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)this.value).ToUInt16(provider);
        public uint ToUInt32(IFormatProvider provider) => ((IConvertible)this.value).ToUInt32(provider);
        public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)this.value).ToUInt64(provider);
    }
}
