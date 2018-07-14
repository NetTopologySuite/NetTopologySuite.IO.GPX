using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO
{
    public static class GpxReader
    {
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings
        {
            CloseInput = false,
        };

        public static (GpxMetadata metadata, Feature[] features) ReadFeatures(TextReader reader, GpxReaderSettings settings, IGeometryFactory geometryFactory)
        {
            return ReadFeatures(reader, settings, new NetTopologySuiteFeatureBuilderVisitor(geometryFactory));
        }

        public static (GpxMetadata metadata, Feature[] features) ReadFeatures(TextReader reader, GpxReaderSettings settings, NetTopologySuiteFeatureBuilderVisitor visitor)
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

        public static void Read(TextReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
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
            using (var xmlReader = XmlReader.Create(reader, Settings, Helpers.GpxNamespace))
            {
                while (xmlReader.ReadToFollowing("gpx", Helpers.GpxNamespace))
                {
                    string version = null;
                    string creator = null;
                    for (bool hasAttribute = xmlReader.MoveToFirstAttribute(); hasAttribute; hasAttribute = xmlReader.MoveToNextAttribute())
                    {
                        switch (xmlReader.Name)
                        {
                            case "version":
                                version = xmlReader.Value;
                                break;

                            case "creator":
                                creator = xmlReader.Value;
                                break;
                        }
                    }

                    if (version != "1.1" || creator is null)
                    {
                        xmlReader.Skip();
                    }

                    bool readMetadata = false;
                    bool readExtensions = false;
                    while (ReadTo(xmlReader, XmlNodeType.Element, XmlNodeType.EndElement))
                    {
                        switch (xmlReader.Name)
                        {
                            // ideally, it should all be in this order, since the XSD validation
                            // would fail otherwise, but whatever.
                            case "metadata" when !readMetadata:
                                ReadMetadata(xmlReader, settings, visitor);
                                readMetadata = true;
                                break;

                            case "wpt":
                                ReadWaypoint(xmlReader, settings, visitor);
                                break;

                            case "rte":
                                ReadRoute(xmlReader, settings, visitor);
                                break;

                            case "trk":
                                ReadTrack(xmlReader, settings, visitor);
                                break;

                            case "extensions" when !readExtensions:
                                visitor.ConvertMetadataExtensionElement((XElement)XNode.ReadFrom(xmlReader));
                                readExtensions = true;
                                break;
                        }
                    }
                }
            }
        }

        private static bool ReadTo(XmlReader xmlReader, XmlNodeType trueNodeType, XmlNodeType falseNodeType)
        {
            while (xmlReader.Read())
            {
                var nt = xmlReader.NodeType;
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

        private static void ReadMetadata(XmlReader xmlReader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(xmlReader);
            var metadata = GpxMetadata.Load(element, settings, visitor.ConvertMetadataExtensionElement);
            visitor.VisitMetadata(metadata);
        }

        private static void ReadWaypoint(XmlReader xmlReader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(xmlReader);
            var waypoint = GpxWaypoint.Load(element, settings, visitor.ConvertWaypointExtensionElement);
            visitor.VisitWaypoint(waypoint);
        }

        private static void ReadRoute(XmlReader xmlReader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(xmlReader);
            var route = GpxRoute.Load(element, settings, visitor.ConvertRouteExtensionElement, visitor.ConvertWaypointExtensionElement);
            visitor.VisitRoute(route);
        }

        private static void ReadTrack(XmlReader xmlReader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(xmlReader);
            var track = GpxTrack.Load(element, settings, visitor.ConvertTrackExtensionElement, visitor.ConvertTrackSegmentExtensionElement, visitor.ConvertWaypointExtensionElement);
            visitor.VisitTrack(track);
        }
    }
}
