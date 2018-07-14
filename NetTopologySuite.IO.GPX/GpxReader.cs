using System;
using System.Xml;
using System.Xml.Linq;

using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public static class GpxReader
    {
        public static (GpxMetadata metadata, Feature[] features) ReadFeatures(XmlReader reader, GpxReaderSettings settings, IGeometryFactory geometryFactory)
        {
            if (geometryFactory is null)
            {
                throw new ArgumentNullException(nameof(geometryFactory));
            }

            return ReadFeatures(reader, settings, new NetTopologySuiteFeatureBuilderVisitor(geometryFactory));
        }

        public static (GpxMetadata metadata, Feature[] features) ReadFeatures(XmlReader reader, GpxReaderSettings settings, NetTopologySuiteFeatureBuilderVisitor visitor)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            Read(reader, settings, visitor);
            return visitor.Terminate();
        }

        public static void Read(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            settings = settings ?? new GpxReaderSettings();
            while (reader.ReadToFollowing("gpx", Helpers.GpxNamespace))
            {
                string version = null;
                string creator = null;
                for (bool hasAttribute = reader.MoveToFirstAttribute(); hasAttribute; hasAttribute = reader.MoveToNextAttribute())
                {
                    switch (reader.Name)
                    {
                        case "version":
                            version = reader.Value;
                            break;

                        case "creator":
                            creator = reader.Value;
                            break;
                    }
                }

                if (version != "1.1" || creator is null)
                {
                    reader.Skip();
                }

                bool readMetadata = false;
                bool readExtensions = false;
                while (ReadTo(reader, XmlNodeType.Element, XmlNodeType.EndElement))
                {
                    switch (reader.Name)
                    {
                        // ideally, it should all be in this order, since the XSD validation
                        // would fail otherwise, but whatever.
                        case "metadata" when !readMetadata:
                            ReadMetadata(reader, settings, creator, visitor);
                            readMetadata = true;
                            break;

                        case "wpt":
                            ReadWaypoint(reader, settings, visitor);
                            break;

                        case "rte":
                            ReadRoute(reader, settings, visitor);
                            break;

                        case "trk":
                            ReadTrack(reader, settings, visitor);
                            break;

                        case "extensions" when !readExtensions:
                            visitor.ConvertMetadataExtensionElement((XElement)XNode.ReadFrom(reader));
                            readExtensions = true;
                            break;
                    }
                }
            }
        }

        private static bool ReadTo(XmlReader reader, XmlNodeType trueNodeType, XmlNodeType falseNodeType)
        {
            while (reader.Read())
            {
                var nt = reader.NodeType;
                if (nt == trueNodeType)
                {
                    return true;
                }
                else if (nt == falseNodeType)
                {
                    return false;
                }
            }

            return false;
        }

        private static void ReadMetadata(XmlReader reader, GpxReaderSettings settings, string creator, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var metadata = GpxMetadata.Load(element, settings, creator, visitor.ConvertMetadataExtensionElement);
            visitor.VisitMetadata(metadata);
        }

        private static void ReadWaypoint(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var waypoint = GpxWaypoint.Load(element, settings, visitor.ConvertWaypointExtensionElement);
            visitor.VisitWaypoint(waypoint);
        }

        private static void ReadRoute(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var route = GpxRoute.Load(element, settings, visitor.ConvertRouteExtensionElement, visitor.ConvertWaypointExtensionElement);
            visitor.VisitRoute(route);
        }

        private static void ReadTrack(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var track = GpxTrack.Load(element, settings, visitor.ConvertTrackExtensionElement, visitor.ConvertTrackSegmentExtensionElement, visitor.ConvertWaypointExtensionElement);
            visitor.VisitTrack(track);
        }
    }
}
