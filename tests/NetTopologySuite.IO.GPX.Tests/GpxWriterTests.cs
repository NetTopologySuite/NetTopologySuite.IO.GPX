using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using Org.XmlUnit.Builder;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxWriterTests
    {
        [Theory]
        [MemberData(nameof(RoundTripSafeSamples))]
        public void RoundTripTest(string path)
        {
            var (metadata, features, extensions) = GpxReader.ReadFeatures(XmlReader.Create(path), null, GeometryFactory.Default);
            using (var ms = new MemoryStream())
            {
                var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false };
                using (var wr = XmlWriter.Create(ms, writerSettings))
                {
                    GpxWriter.Write(wr, null, metadata, features, extensions);
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

        public static object[][] RoundTripSafeSamples => Array.ConvertAll(Directory.GetFiles("RoundTripSafeSamples", "*.gpx", SearchOption.TopDirectoryOnly), fl => new object[] { fl });

        [Fact]
        [GitHubIssue(36)]
        public void CustomRootNamespacesShouldBeAbleToBeSpecified()
        {
            string expected = @"<gpx version='1.1' creator='Creator'
                    xmlns='http://www.topografix.com/GPX/1/1'
                    xmlns:ABC='http://www.mycustomnamespace.net/xml'>
                <extensions>
                    <ABC:Key>Value</ABC:Key>
                </extensions>
            </gpx>";

            using (var ms = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false };

                using (var wr = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    XNamespace ns = "http://www.mycustomnamespace.net/xml";

                    var gpxWriterSettings = new GpxWriterSettings();
                    gpxWriterSettings.Namespaces["ABC"] = new Uri(ns.NamespaceName);

                    var gpxMetadata = new GpxMetadata("Creator");
                    var features = new List<IFeature>();

                    var extensions = new List<XElement>()
                    {
                        new XElement(ns + "Key", "Value"),
                    };

                    GpxWriter.Write(wr, gpxWriterSettings, gpxMetadata, features, extensions);
                }

                ms.Position = 0;
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
    }
}
