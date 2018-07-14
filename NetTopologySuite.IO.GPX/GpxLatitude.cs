using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    public readonly struct GpxLatitude : IEquatable<GpxLatitude>, IComparable<GpxLatitude>, IComparable, IFormattable, IConvertible
    {
        private readonly double value;

        public GpxLatitude(double val)
        {
            if (!(Math.Abs(val) <= 90))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.value = val;
        }

        public static bool operator ==(GpxLatitude lat1, GpxLatitude lat2) => lat1.value == lat2.value;

        public static bool operator !=(GpxLatitude lat1, GpxLatitude lat2) => lat1.value != lat2.value;

        public static bool operator <(GpxLatitude lat1, GpxLatitude lat2) => lat1.value < lat2.value;

        public static bool operator <=(GpxLatitude lat1, GpxLatitude lat2) => lat1.value <= lat2.value;

        public static bool operator >(GpxLatitude lat1, GpxLatitude lat2) => lat1.value >= lat2.value;

        public static bool operator >=(GpxLatitude lat1, GpxLatitude lat2) => lat1.value >= lat2.value;

        public static implicit operator double(GpxLatitude lat) => lat.value;

        public static explicit operator GpxLatitude(double val) => new GpxLatitude(val);

        public override bool Equals(object obj) => obj is GpxLatitude other && this.value == other.value;

        public bool Equals(GpxLatitude other) => this.value == other.value;

        public int CompareTo(GpxLatitude other) => this.value.CompareTo(other.value);

        public int CompareTo(object obj)
        {
            if (!(obj is GpxLatitude other))
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
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be Latitude", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between -90 and +90, inclusive");

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
