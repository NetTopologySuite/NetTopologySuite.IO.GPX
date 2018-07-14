using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

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
            string gpx = File.ReadAllText(gpxPath);
            var gpxElement = XDocument.Parse(gpx).Root;
            using (var stringReader = new StringReader(gpx))
            using (var reader = XmlReader.Create(stringReader))
            {
                var (metadata, features) = GpxReader.ReadFeatures(reader, null, GeometryFactory.Default);
                Assert.Equal(gpxElement.Attribute("creator").Value, metadata.Creator);
            }
        }

        public static object[][] GpxData => Array.ConvertAll(Directory.GetFiles("SampleData", "*.gpx", SearchOption.TopDirectoryOnly), fl => new object[] { fl });
    }
}
