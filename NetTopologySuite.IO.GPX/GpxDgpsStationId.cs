using System;
using System.Runtime.CompilerServices;

namespace NetTopologySuite.IO
{
    public readonly struct GpxDgpsStationId : IEquatable<GpxDgpsStationId>, IComparable<GpxDgpsStationId>, IComparable, IFormattable, IConvertible
    {
        private readonly ushort value;

        public GpxDgpsStationId(ushort val)
        {
            if (!(val < 1024))
            {
                ThrowArgumentOutOfRangeException();
            }

            this.value = val;
        }

        public static bool operator ==(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value == id2.value;

        public static bool operator !=(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value != id2.value;

        public static bool operator <(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value < id2.value;

        public static bool operator <=(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value <= id2.value;

        public static bool operator >(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value >= id2.value;

        public static bool operator >=(GpxDgpsStationId id1, GpxDgpsStationId id2) => id1.value >= id2.value;

        public static implicit operator ushort(GpxDgpsStationId id) => id.value;

        public static explicit operator GpxDgpsStationId(ushort val) => new GpxDgpsStationId(val);

        public override bool Equals(object obj) => obj is GpxDgpsStationId other && this.value == other.value;

        public bool Equals(GpxDgpsStationId other) => this.value == other.value;

        public int CompareTo(GpxDgpsStationId other) => this.value.CompareTo(other.value);

        public int CompareTo(object obj)
        {
            if (!(obj is GpxDgpsStationId other))
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
        private static void ThrowArgumentException() => throw new ArgumentException("Type must be a DGPS station ID", "obj");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("val", "Must be between 0 and 1023, inclusive");

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
