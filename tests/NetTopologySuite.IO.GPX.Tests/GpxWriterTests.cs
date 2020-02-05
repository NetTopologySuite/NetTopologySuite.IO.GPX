using System;
using System.IO;
using System.Linq;
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
            const string DesiredPrefix = "ABC";
            const string NamespaceName = "http://www.example.com/xml";

            string expected = $@"
<gpx xmlns='http://www.topografix.com/GPX/1/1' version='1.1' creator='Creator' xmlns:{DesiredPrefix}= '{NamespaceName}'>
    <extensions>
        <{DesiredPrefix}:Key>Value</{DesiredPrefix}:Key>
    </extensions>
</gpx>";

            XElement parsedRoot;
            using (var ms = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false, NamespaceHandling = NamespaceHandling.OmitDuplicates };
                using (var wr = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    var gpxWriterSettings = new GpxWriterSettings
                    {
                        CommonXmlNamespacesByDesiredPrefix =
                        {
                            [DesiredPrefix] = new Uri(NamespaceName),
                        },
                    };

                    XElement[] extensions = { new XElement(XName.Get("Key", NamespaceName), "Value"), };

                    GpxWriter.Write(wr, gpxWriterSettings, new GpxMetadata("Creator"), Enumerable.Empty<IFeature>(), extensions);
                }

                ms.Position = 0;
                parsedRoot = XDocument.Load(ms).Root;
            }

            // first check that it defines the same content
            var diff = DiffBuilder.Compare(expected)
                                  .NormalizeWhitespace()
                                  .WithTest(parsedRoot)
                                  .IgnoreComments()
                                  .CheckForSimilar()
                                  .Build();
            Assert.False(diff.HasDifferences(), string.Join(Environment.NewLine, diff.Differences));

            // everything above actually passes without the fix for #36 or if we ignore the desired
            // prefix that we're given, so make sure to test that we actually put the namespace
            // declaration on the root with the desired prefix.
            Assert.Equal(DesiredPrefix, parsedRoot.GetPrefixOfNamespace(NamespaceName));

            // prove that, with NamespaceHandling.OmitDuplicates, duplicates are actually omitted.
            var redundantDeclarations = from descendant in parsedRoot.Descendants()
                                        from attribute in descendant.Attributes()
                                        where attribute.IsNamespaceDeclaration && attribute.Value == NamespaceName
                                        select (descendant, attribute);
            Assert.Empty(redundantDeclarations);
        }
    }
}
