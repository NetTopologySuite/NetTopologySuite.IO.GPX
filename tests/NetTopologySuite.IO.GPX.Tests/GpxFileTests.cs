using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Org.XmlUnit.Builder;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxFileTests
    {
        [Theory]
        [MemberData(nameof(RoundTripSafeSamples))]
        public void RoundTripTest(string path)
        {
            var file = GpxFile.ReadFrom(XmlReader.Create(path), null);
            using (var ms = new MemoryStream())
            {
                var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false };
                using (var wr = XmlWriter.Create(ms, writerSettings))
                {
                    file.WriteTo(wr, null);
                }

                ms.Position = 0;
                byte[] expected = File.ReadAllBytes(path);
                var diff = DiffBuilder.Compare(expected)
                                      .NormalizeWhitespace()
                                      .WithTest(ms)
                                      .IgnoreComments()
                                      .CheckForSimilar()
                                      .Build();

                // note that this is not a guarantee in the general case.  the inputs here have all been
                // slightly tweaked such that it should succeed for our purposes.
                Assert.False(diff.HasDifferences(), string.Join(Environment.NewLine, diff.Differences));
            }
        }

        [Theory]
        [MemberData(nameof(RoundTripSafeSamples))]
        public void RoundTripTestUsingText(string path)
        {
            string expected = File.ReadAllText(path);
            var file = GpxFile.Parse(expected, null);
            string actual = file.BuildString(null);
            var diff = DiffBuilder.Compare(expected)
                                  .NormalizeWhitespace()
                                  .WithTest(actual)
                                  .IgnoreComments()
                                  .CheckForSimilar()
                                  .Build();

            // note that this is not a guarantee in the general case.  the inputs here have all been
            // slightly tweaked such that it should succeed for our purposes.
            Assert.False(diff.HasDifferences(), string.Join(Environment.NewLine, diff.Differences));
        }

        public static object[][] RoundTripSafeSamples => Array.ConvertAll(Directory.GetFiles("RoundTripSafeSamples", "*.gpx", SearchOption.TopDirectoryOnly), fl => new object[] { fl });

        [Fact]
        public void TestParseUsingInlineText()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata />
    <wpt lat='0.1' lon='2.3' />
    <rte><rtept lat='4.5' lon='6.7' /></rte>
    <trk><trkseg><trkpt lat='8.9' lon='10.11' /></trkseg></trk>
    <extensions><someArbitraryElement xmlns='http://www.example.com' data='x' /></extensions>
</gpx>
";
            var file = GpxFile.Parse(GpxText, null);
            Assert.Equal("airbreather", file.Metadata.Creator);
            Assert.True(file.Metadata.IsTrivial, "Metadata should be considered trivial, even if it's specified as blank.");

            var actualWaypoint = Assert.Single(file.Waypoints);
            Assert.Equal(0.1, actualWaypoint.Latitude);
            Assert.Equal(2.3, actualWaypoint.Longitude);

            var actualRoute = Assert.Single(file.Routes);
            var actualRoutePoint = Assert.Single(actualRoute.Waypoints);
            Assert.Equal(4.5, actualRoutePoint.Latitude);
            Assert.Equal(6.7, actualRoutePoint.Longitude);

            var actualTrack = Assert.Single(file.Tracks);
            var actualTrackSegment = Assert.Single(actualTrack.Segments);
            var actualTrackPoint = Assert.Single(actualTrackSegment.Waypoints);
            Assert.Equal(8.9, actualTrackPoint.Latitude);
            Assert.Equal(10.11, actualTrackPoint.Longitude);

            Assert.NotNull(file.Extensions);
            var actualExtensionElements = Assert.IsType<XElement[]>(file.Extensions);
            var actualExtensionElement = Assert.Single(actualExtensionElements);
            Assert.Equal(XName.Get("someArbitraryElement", "http://www.example.com"), actualExtensionElement.Name);
            Assert.Equal("x", actualExtensionElement.Attribute("data")?.Value);
        }

        [Fact]
        public void DefaultInstanceShouldBeValid()
        {
            var file = new GpxFile();
            using (var ms = new MemoryStream())
            {
                var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false };
                using (var wr = XmlWriter.Create(ms, writerSettings))
                {
                    file.WriteTo(wr, null);
                }

                ms.Position = 0;
                file = GpxFile.ReadFrom(XmlReader.Create(ms), null);
                Assert.NotNull(file);
                Assert.Equal("NetTopologySuite.IO.GPX", file.Metadata.Creator);
                Assert.True(file.Metadata.IsTrivial);
                Assert.Empty(file.Waypoints);
                Assert.Empty(file.Routes);
                Assert.Empty(file.Tracks);
                Assert.Null(file.Extensions);
            }
        }

        [Fact]
        public void GitHubIssue15RegressionTest()
        {
            var expectedWaypoint = new GpxWaypoint(new GpxLongitude(0.00001), new GpxLatitude(double.Epsilon), -double.Epsilon);
            var file = new GpxFile
            {
                Waypoints = { expectedWaypoint },
            };

            file = GpxFile.Parse(file.BuildString(null), null);

            var actualWaypoint = Assert.Single(file.Waypoints);

            Assert.Equal(expectedWaypoint.Longitude, actualWaypoint.Longitude);
            Assert.Equal(expectedWaypoint.Latitude, actualWaypoint.Latitude);
            Assert.Equal(expectedWaypoint.ElevationInMeters, actualWaypoint.ElevationInMeters);
        }
    }
}
