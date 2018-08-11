﻿using System;
using System.IO;
using System.Text;
using System.Xml;

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
    }
}
