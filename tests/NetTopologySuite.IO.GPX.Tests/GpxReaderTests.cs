﻿using System;
using System.IO;
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
        [Regression]
        [InlineData("copyright-regression-no-timezone.gpx", 2040)]
        [InlineData("copyright-regression-utc.gpx", 2001)]
        [InlineData("copyright-regression-real-offset.gpx", 2009)]
        [InlineData("copyright-regression-insane-year-1.gpx", -2)]
        [InlineData("copyright-regression-insane-year-2.gpx", 20072007)]
        public void CopyrightYearShouldAllowVariousLegalValues(string gpxFileName, int expectedYear)
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
        [Regression]
        [GitHubIssue(18)]
        public void MinimalWhitespaceShouldNotCauseFeaturesToGetSkipped()
        {
            using (var xmlReader = XmlReader.Create(Path.Join("RoundTripSafeSamples", "github-issue18-regression.gpx")))
            {
                var (_, features, _) = GpxReader.ReadFeatures(xmlReader, null, GeometryFactory.Default);
                Assert.Equal(3, features.Length);
            }
        }
    }
}
