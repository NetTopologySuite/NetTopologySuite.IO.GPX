using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using NetTopologySuite.Geometries;

using Xunit;

namespace NetTopologySuite.IO
{
    // the more "interesting" tests (the round-trip ones) are in GpxWriterTests.
    public sealed class GpxReaderTests
    {
        [Theory]
        [MemberData(nameof(AllSampleGpxFiles))]
        public void ReadSmokeTest(string gpxPath)
        {
            string gpx = File.ReadAllText(gpxPath);
            var gpxElement = XDocument.Parse(gpx).Root;
            using (var stringReader = new StringReader(gpx))
            using (var reader = XmlReader.Create(stringReader))
            {
                var (metadata, features, extensions) = GpxReader.ReadFeatures(reader, null, GeometryFactory.Default);
                Assert.Equal(gpxElement.Attribute("creator").Value, metadata.Creator);
            }
        }

        [Theory]
        [InlineData("copyright-regression-no-timezone.gpx", 2040)]
        [InlineData("copyright-regression-utc.gpx", 2001)]
        [InlineData("copyright-regression-real-offset.gpx", 2009)]
        [InlineData("copyright-regression-insane-year-1.gpx", -2)]
        [InlineData("copyright-regression-insane-year-2.gpx", 20072007)]
        public void CopyrightYearRegressionTest(string gpxFileName, int expectedYear)
        {
            using (var reader = XmlReader.Create(Path.Combine("CopyrightYearRegressionSamples", gpxFileName)))
            {
                var (metadata, _, _) = GpxReader.ReadFeatures(reader, null, GeometryFactory.Default);
                Assert.True(metadata.Copyright?.Year.HasValue);
                int value = metadata.Copyright.Year.GetValueOrDefault();
                Assert.Equal(expectedYear, value);
            }
        }

        public static object[][] AllSampleGpxFiles => Array.ConvertAll(Directory.GetFiles(".", "*.gpx", SearchOption.AllDirectories), fl => new object[] { fl });

        [Fact]
        public void GitHubIssue15RegressionTest()
        {
            using (var ms = new MemoryStream())
            {
                using (var textWriter = new StreamWriter(ms, Encoding.UTF8, 4096, true))
                using (var xmlWriter = XmlWriter.Create(textWriter))
                {
                    GpxWriter.Write(xmlWriter,
                                    null,
                                    new GpxMetadata("creator is irrelevant"),
                                    new[] { new GpxWaypoint(new GpxLongitude(0.00001), new GpxLatitude(double.Epsilon), -double.Epsilon) },
                                    Enumerable.Empty<GpxRoute>(),
                                    Enumerable.Empty<GpxTrack>(),
                                    null);
                }

                ms.Position = 0;
                using (var textReader = new StreamReader(ms, Encoding.UTF8, false, 4096, true))
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    var (_, features, _) = GpxReader.ReadFeatures(xmlReader, null, GeometryFactory.Default);
                    var coord = features[0].Geometry.Coordinate;
                    Assert.Equal(0.00001, coord.X);
                    Assert.Equal(double.Epsilon, coord.Y);
                    Assert.Equal(-double.Epsilon, coord.Z);
                }
            }
        }
    }
}
