using System;
using System.IO;

using NetTopologySuite.Geometries;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxReaderTests
    {
        [Theory]
        [MemberData(nameof(GpxData))]
        public void Read(string gpxPath) => GpxReader.ReadFeatures(new StreamReader(gpxPath), null, GeometryFactory.Default);

        public static object[][] GpxData => Array.ConvertAll(Directory.GetFiles("SampleData", "*.gpx", SearchOption.TopDirectoryOnly), fl => new object[] { fl });
    }
}
