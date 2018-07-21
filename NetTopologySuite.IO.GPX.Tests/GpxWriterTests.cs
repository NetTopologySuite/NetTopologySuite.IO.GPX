using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxWriterTests
    {
        [Fact]
        public void Write()
        {
            string path = Path.Combine("SampleData", "St_Louis_Zoo_sample.gpx");
            string originalCanonicalXml;
            {
                var doc = new XmlDocument();
                doc.Load(path);
                originalCanonicalXml = Canonicalize(doc);
            }

            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, CloseOutput = false };
            var (metadata, features) = GpxReader.ReadFeatures(XmlReader.Create(path), null, GeometryFactory.Default);
            var waypoints = new List<GpxWaypoint>();
            foreach (var feature in features)
            {
                if (feature.Geometry is IPoint)
                {
                    waypoints.Add((GpxWaypoint)feature.Attributes["wpt"]);
                }
            }

            string roundTrippedXml;
            using (var ms = new MemoryStream())
            {
                using (var wr = XmlWriter.Create(ms, writerSettings))
                {
                    GpxWriter.Write(wr, null, metadata, waypoints, null, null);
                }

                ms.Position = 0;
                var doc = new XmlDocument();
                doc.Load(new StreamReader(ms, Encoding.UTF8));
                roundTrippedXml = Canonicalize(doc);
            }

            // note that this is not a guarantee in the general case.  the input has been slightly
            // tweaked such that it should succeed for our purposes:
            // - timestamp was marked as UTC
            // - xsi:schemaLocation attribute was removed from the gpx element
            Assert.Equal(originalCanonicalXml, roundTrippedXml);
        }

        private static string Canonicalize(XmlDocument doc)
        {
            var t = new XmlDsigC14NTransform();
            t.LoadInput(doc);
            using (var stream = (Stream)t.GetOutput())
            {
                var decoder = Encoding.UTF8.GetDecoder();
                var sb = new StringBuilder();
                Span<byte> buf = stackalloc byte[100];
                Span<char> chr = stackalloc char[100];
                int cnt;
                while ((cnt = stream.Read(buf)) != 0)
                {
                    int decoded = decoder.GetChars(buf, chr, false);
                    sb.Append(chr.Slice(0, decoded));
                }

                return sb.ToString();
            }
        }
    }
}
