using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using GeoAPI.Geometries;
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
            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, CloseOutput = false };
            var (metadata, features) = GpxReader.ReadFeatures(XmlReader.Create(path), null, GeometryFactory.Default);
            var waypoints = new List<GpxWaypoint>();
            var routes = new List<GpxRoute>();
            var tracks = new List<GpxTrack>();
            foreach (var feature in features)
            {
                switch (feature.Geometry)
                {
                    case IPoint _:
                        waypoints.Add((GpxWaypoint)feature.Attributes["wpt"]);
                        break;

                    case ILineString _:
                        routes.Add((GpxRoute)feature.Attributes["rte"]);
                        break;

                    case IMultiLineString _:
                        tracks.Add((GpxTrack)feature.Attributes["trk"]);
                        break;
                }
            }

            using (var ms = new MemoryStream())
            {
                using (var wr = XmlWriter.Create(ms, writerSettings))
                {
                    GpxWriter.Write(wr, null, metadata, waypoints, routes, tracks);
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
