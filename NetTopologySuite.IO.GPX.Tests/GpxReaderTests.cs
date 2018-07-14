using System;
using System.IO;
using System.Xml;
using NetTopologySuite.Geometries;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxReaderTests
    {
        [Theory]
        [MemberData(nameof(GpxData))]
        public void Read(string gpxPath)
        {
            using (var reader = XmlReader.Create(gpxPath))
            {
                GpxReader.ReadFeatures(reader, null, GeometryFactory.Default);
            }
        }

        public static object[][] GpxData => Array.ConvertAll(Directory.GetFiles("SampleData", "*.gpx", SearchOption.TopDirectoryOnly), fl => new object[] { fl });
    }
}
