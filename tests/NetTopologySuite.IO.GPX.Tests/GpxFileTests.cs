﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Org.XmlUnit.Builder;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxFileTests
    {
        private readonly Random random = new Random(12345);

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
        public void RoundTripTestStartingFromModelObjects()
        {
            var file1 = new GpxFile();
            file1.Metadata = DataObjectBuilders.RandomGpxMetadata(random);
            for (int i = 0, cnt = random.Next(5, 10); i < cnt; i++)
            {
                file1.Waypoints.Add(DataObjectBuilders.RandomWaypoint(random));
            }

            for (int i = 0, cnt = random.Next(5, 10); i < cnt; i++)
            {
                file1.Routes.Add(DataObjectBuilders.RandomRoute(random));
            }

            for (int i = 0, cnt = random.Next(5, 10); i < cnt; i++)
            {
                file1.Tracks.Add(DataObjectBuilders.RandomTrack(random));
            }

            file1.Extensions = DataObjectBuilders.RandomExtensions(random);

            var file2 = GpxFile.Parse(file1.BuildString(null), null);

            Assert.Equal(file1.Metadata, file2.Metadata);
            Assert.Equal(file1.Waypoints.AsEnumerable(), file2.Waypoints.AsEnumerable());
            Assert.Equal(file1.Routes.AsEnumerable(), file2.Routes.AsEnumerable());
            Assert.Equal(file1.Tracks.AsEnumerable(), file2.Tracks.AsEnumerable());
            Assert.StrictEqual(file1.Extensions, file2.Extensions);
        }

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
            var actualExtensionElements = Assert.IsType<ImmutableXElementContainer>(file.Extensions);
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
        [Regression]
        [GitHubIssue(15)]
        public void RoundTripForValuesVeryNearZeroShouldSucceed()
        {
            var expectedWaypoint = new GpxWaypoint(new GpxLongitude(0.00001), new GpxLatitude(double.Epsilon), -double.Epsilon);
            var file = new GpxFile
            {
                Waypoints = { expectedWaypoint },
            };

            file = GpxFile.Parse(file.BuildString(null), null);

            var actualWaypoint = Assert.Single(file.Waypoints);
            Assert.Equal(expectedWaypoint, actualWaypoint);
        }

        [Theory]
        [Regression]
        [GitHubIssue(24)]
        [InlineData("<wpt lat='1' lon='1'><ele>+Infinity</ele></wpt>")]
        [InlineData("<rte><rtept lat='1' lon='1'><ele>-Infinity</ele></rtept></rte>")]
        [InlineData("<trk><trkseg><trkpt lat='1' lon='1'><ele>Infinity</ele></trkpt></trkseg></trk>")]
        public void ParseWithInfiniteWaypointElevationShouldFail(string inner)
        {
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse($"<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>{inner}</gpx>", null));
        }

        [Theory]
        [InlineData("<gpx xmlns='http://www.topografix.com/GPX/1/1' />")]
        [InlineData("<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' />")]
        [InlineData("<gpx xmlns='http://www.topografix.com/GPX/1/1' creator='someone' />")]
        [InlineData("<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.2' creator='someone' />")]
        public void RequiredGpxAttributesMustBePresentByDefault(string gpx)
        {
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(gpx, null));
        }

        [Fact]
        [GitHubIssue(23)]
        public void MissingCreatorShouldFillInDefault_OptIn()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' />
";
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(GpxText, null));

            var settings = new GpxReaderSettings { DefaultCreatorIfMissing = "Legacy IHM" };
            var file = GpxFile.Parse(GpxText, settings);
            Assert.Equal("Legacy IHM", file.Metadata.Creator);
        }

        [Fact]
        [GitHubIssue(27)]
        public void MissingVersionShouldBeValid_OptIn()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' creator='someone' />
";
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(GpxText, null));

            var settings = new GpxReaderSettings { IgnoreVersionAttribute = true };
            var file = GpxFile.Parse(GpxText, settings);
            Assert.Equal("someone", file.Metadata.Creator);
        }

        [Fact]
        [GitHubIssue(28)]
        public void DifferentVersionShouldBeValid_OptIn()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.2' creator='someone' />
";
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(GpxText, null));

            var settings = new GpxReaderSettings { IgnoreVersionAttribute = true };
            var file = GpxFile.Parse(GpxText, settings);
            Assert.Equal("someone", file.Metadata.Creator);
        }

        [Fact]
        [GitHubIssue(29)]
        public void BadMetadataCreationTimeShouldBeValid_OptIn()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata>
        <time>0000-00-00T00:00:00Z</time>
    </metadata>
</gpx>
";
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(GpxText, null));

            var settings = new GpxReaderSettings { IgnoreBadDateTime = true };
            var file = GpxFile.Parse(GpxText, settings);
            Assert.Null(file.Metadata.CreationTimeUtc);
        }

        [Fact]
        [GitHubIssue(29)]
        public void BadWaypointTimestampShouldBeValid_OptIn()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata />
    <wpt lat='0.1' lon='2.3'>
        <time>HELLO</time>
    </wpt>
</gpx>
";
            Assert.ThrowsAny<XmlException>(() => GpxFile.Parse(GpxText, null));

            var settings = new GpxReaderSettings { IgnoreBadDateTime = true };
            var file = GpxFile.Parse(GpxText, settings);
            Assert.Null(file.Waypoints[0].TimestampUtc);
        }

        [Fact]
        public void TimestampsShouldBeInterpretedInGivenTimeZone()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata>
        <time>1234-05-06T07:08:09</time>
    </metadata>
    <wpt lat='0.1' lon='2.3'>
        <time>5432-10-10T11:22:33</time>
    </wpt>
</gpx>
";
            var inputTimeZone = TimeZoneInfo.CreateCustomTimeZone("meh", TimeSpan.FromHours(3.5), "meh", "meh");
            var settings = new GpxReaderSettings { TimeZoneInfo = inputTimeZone };
            var file = GpxFile.Parse(GpxText, settings);

            // we told it that our times are in a time zone that's at +3.5 hours from UTC, and the
            // data is stored UTC, so we should see -3.5 from what's written.
            Assert.Equal(new DateTime(1234, 05, 06, 03, 38, 09), file.Metadata.CreationTimeUtc.GetValueOrDefault());
            Assert.Equal(new DateTime(5432, 10, 10, 07, 52, 33), file.Waypoints[0].TimestampUtc.GetValueOrDefault());
        }

        [Fact]
        [Regression]
        [GitHubIssue(30)]
        public void TimestampsShouldBeWrittenInGivenTimeZone()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata>
        <time>1234-05-06T07:08:09</time>
    </metadata>
    <wpt lat='0.1' lon='2.3'>
        <time>5432-10-10T11:22:33</time>
    </wpt>
</gpx>
";
            var outputTimeZone = TimeZoneInfo.CreateCustomTimeZone("meh", TimeSpan.FromHours(3.5), "meh", "meh");
            var settings = new GpxWriterSettings { TimeZoneInfo = outputTimeZone };
            string text = GpxFile.Parse(GpxText, null).BuildString(settings);

            // we read in times assuming UTC, and then asked it to write them out in a time zone
            // that's at +3.5 hours from UTC, so we should see +3.5 from what's written, *AND* with
            // an offset (at least at the time of writing, we always write out the most unambiguous
            // times that it seems like the framework allows).
            Assert.Contains("1234-05-06T10:38:09+03:30", text);
            Assert.Contains("5432-10-10T14:52:33+03:30", text);
        }

        [Fact]
        [Regression]
        [GitHubIssue(31)]
        public void TimestampsShouldPreserveFractionalSecondsWithinDefinedPrecision()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <metadata>
        <time>1234-05-06T07:08:09.7654321</time>
    </metadata>
    <wpt lat='0.1' lon='2.3'>
        <time>5432-10-10T11:22:33.87654321</time>
    </wpt>
    <wpt lat='4.5' lon='6.7'>
        <time>1111-11-11T11:11:11.12345</time>
    </wpt>
</gpx>
";
            string text = GpxFile.Parse(GpxText, null). BuildString(null);

            Assert.Contains("1234-05-06T07:08:09.7654321Z", text);
            Assert.Contains("5432-10-10T11:22:33.8765432Z", text); // DateTime resolution is 100ns, so the value gets rounded to 7 digits
            Assert.Contains("1111-11-11T11:11:11.12345Z", text); // don't output extra zeroes
        }

        [Fact]
        [Regression]
        [GitHubIssue(33)]
        public void ExtensionsElementInGpxNamespaceShouldNotRepeatNamespaceSpecByDefault()
        {
            const string GpxText = @"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='airbreather'>
    <extensions><myTest someData='4' /></extensions>
</gpx>
";
            string text = GpxFile.Parse(GpxText, null).BuildString(null);

            // be careful when using regex to match XML.
            var namespaceWasRepeatedRegex = new Regex("<extensions>.*topografix.*</extensions>", RegexOptions.Singleline);
            Assert.DoesNotMatch(namespaceWasRepeatedRegex, text);
        }
    }
}
