using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    public readonly struct GpxLatitude : IEquatable<GpxLatitude>, IComparable<GpxLatitude>, IComparable, IFormattable, IConvertible
    {
        public readonly double Value;

        public GpxLatitude(double val)
        {
            if (!(Math.Abs(val) <= 90))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.Value = val;
        }

        public static bool operator ==(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value == lat2.Value;

        public static bool operator !=(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value != lat2.Value;

        public static bool operator <(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value < lat2.Value;

        public static bool operator <=(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value <= lat2.Value;

        public static bool operator >(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value >= lat2.Value;

        public static bool operator >=(GpxLatitude lat1, GpxLatitude lat2) => lat1.Value >= lat2.Value;

        public static implicit operator double(GpxLatitude lat) => lat.Value;

        public static explicit operator GpxLatitude(double val) => new GpxLatitude(val);

        public override bool Equals(object obj) => obj is GpxLatitude other && this.Value == other.Value;

        public bool Equals(GpxLatitude other) => this.Value == other.Value;

        public int CompareTo(GpxLatitude other) => this.Value.CompareTo(other.Value);

        public int CompareTo(object obj)
        {
            if (!(obj is GpxLatitude other))
            {
                ThrowArgumentException();
                return 0;
            }

            return this.Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => this.Value.ToString();

        public string ToString(string format, IFormatProvider formatProvider) => this.Value.ToString(format, formatProvider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be Latitude", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between -90 and +90, inclusive");

        public TypeCode GetTypeCode() => this.Value.GetTypeCode();
        public bool ToBoolean(IFormatProvider provider) => ((IConvertible)this.Value).ToBoolean(provider);
        public byte ToByte(IFormatProvider provider) => ((IConvertible)this.Value).ToByte(provider);
        public char ToChar(IFormatProvider provider) => ((IConvertible)this.Value).ToChar(provider);
        public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)this.Value).ToDateTime(provider);
        public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)this.Value).ToDecimal(provider);
        public double ToDouble(IFormatProvider provider) => ((IConvertible)this.Value).ToDouble(provider);
        public short ToInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToInt16(provider);
        public int ToInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToInt32(provider);
        public long ToInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToInt64(provider);
        public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)this.Value).ToSByte(provider);
        public float ToSingle(IFormatProvider provider) => ((IConvertible)this.Value).ToSingle(provider);
        public string ToString(IFormatProvider provider) => this.Value.ToString(provider);
        public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)this.Value).ToType(conversionType, provider);
        public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt16(provider);
        public uint ToUInt32(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt32(provider);
        public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)this.Value).ToUInt64(provider);
    }
}
