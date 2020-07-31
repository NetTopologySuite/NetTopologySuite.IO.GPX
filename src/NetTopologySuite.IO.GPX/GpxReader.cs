using System;
using System.Xml;
using System.Xml.Linq;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Provides methods that read in GPX data from a <see cref="XmlReader"/>.
    /// </summary>
    public static class GpxReader
    {
        /// <summary>
        /// Reads in NTS <see cref="Feature"/>s and GPX metadata from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> to read from.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxReaderSettings"/> instance to use to control how GPX instances get
        /// read in, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <param name="geometryFactory">
        /// The <see cref="GeometryFactory"/> instance to use when creating the individual
        /// <see cref="Feature.Geometry"/> elements.
        /// </param>
        /// <returns>
        /// The <see cref="Feature"/> instances that represent the top-level GPX data elements, as
        /// well as the <see cref="GpxMetadata"/> for the GPX file and the top-level extension
        /// content from the file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="reader"/> or <paramref name="geometryFactory"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="XmlException">
        /// Thrown when <paramref name="reader"/> does not specify a valid GPX file (i.e., cases
        /// where XSD schema validation would fail, and/or some values are <b>well</b> outside of
        /// the slightly stricter, but still completely reasonable, limits imposed by the idiomatic
        /// .NET data types above and beyond the XSD limits).
        /// </exception>
        public static (GpxMetadata metadata, Feature[] features, object extensions) ReadFeatures(XmlReader reader, GpxReaderSettings settings, GeometryFactory geometryFactory)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (geometryFactory is null)
            {
                throw new ArgumentNullException(nameof(geometryFactory));
            }

            var visitor = new NetTopologySuiteFeatureBuilderGpxVisitor(geometryFactory);
            Read(reader, settings, visitor);
            return visitor.Terminate();
        }

        /// <summary>
        /// Processes a GPX file, invoking callbacks on a <see cref="GpxVisitorBase"/> instance as
        /// elements are observed.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> to read from.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxReaderSettings"/> instance to use to control how GPX instances get
        /// read in, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <param name="visitor">
        /// The <see cref="GpxVisitorBase"/> instance that will receive callbacks.
        /// </param>
        /// <remarks>
        /// This method is the "core" reading method; everything else builds off of this.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="reader"/> or <paramref name="visitor"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="XmlException">
        /// Thrown when <paramref name="reader"/> does not specify a valid GPX file (i.e., cases
        /// where XSD schema validation would fail, and/or some values are <b>well</b> outside of
        /// the slightly stricter, but still completely reasonable, limits imposed by the idiomatic
        /// .NET data types above and beyond the XSD limits).
        /// </exception>
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
                string creator = settings.DefaultCreatorIfMissing;
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

                if (version != "1.1" && !settings.IgnoreVersionAttribute)
                {
                    throw new XmlException("'version' must be '1.1'");
                }

                if (creator is null)
                {
                    throw new XmlException("'creator' must be specified");
                }

                if (!ReadTo(reader, XmlNodeType.Element, XmlNodeType.EndElement))
                {
                    visitor.VisitMetadata(new GpxMetadata(creator));
                    break;
                }

                bool expectingMetadata = true;
                bool expectingExtensions = true;
                do
                {
                    if (expectingMetadata)
                    {
                        expectingMetadata = false;
                        if (reader.Name == "metadata")
                        {
                            ReadMetadata(reader, settings, creator, visitor);
                            if (!ReadTo(reader, XmlNodeType.Element, XmlNodeType.EndElement))
                            {
                                break;
                            }
                        }
                        else
                        {
                            visitor.VisitMetadata(new GpxMetadata(creator));
                        }
                    }

                    switch (reader.Name)
                    {
                        // ideally, it should all be in this order, since the XSD validation
                        // would fail otherwise, but whatever.
                        case "wpt":
                            ReadWaypoint(reader, settings, visitor);
                            break;

                        case "rte":
                            ReadRoute(reader, settings, visitor);
                            break;

                        case "trk":
                            ReadTrack(reader, settings, visitor);
                            break;

                        case "extensions" when expectingExtensions:
                            expectingExtensions = false;
                            var extensionElement = (XElement)XNode.ReadFrom(reader);
                            object extensions = settings.ExtensionReader.ConvertGpxExtensionElement(extensionElement.Elements());
                            if (!(extensions is null))
                            {
                                visitor.VisitExtensions(extensions);
                            }

                            break;

                        case string _ when settings.IgnoreUnexpectedChildrenOfTopLevelElement:
                            reader.Skip();
                            break;

                        default:
                            throw new XmlException($"Unexpected xml node '{reader.Name}'");
                    }
                }
                while (ReadTo(reader, XmlNodeType.Element, XmlNodeType.EndElement));
            }
        }

        private static bool ReadTo(XmlReader reader, XmlNodeType trueNodeType, XmlNodeType falseNodeType)
        {
            do
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
            while (reader.Read());

            return false;
        }

        private static void ReadMetadata(XmlReader reader, GpxReaderSettings settings, string creator, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var metadata = GpxMetadata.Load(element, settings, creator);
            visitor.VisitMetadata(metadata);
        }

        private static void ReadWaypoint(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var waypoint = GpxWaypoint.Load(element, settings, settings.ExtensionReader.ConvertWaypointExtensionElement);
            visitor.VisitWaypoint(waypoint);
        }

        private static void ReadRoute(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var route = GpxRoute.Load(element, settings);
            visitor.VisitRoute(route);
        }

        private static void ReadTrack(XmlReader reader, GpxReaderSettings settings, GpxVisitorBase visitor)
        {
            var element = (XElement)XNode.ReadFrom(reader);
            var track = GpxTrack.Load(element, settings);
            visitor.VisitTrack(track);
        }
    }
}
