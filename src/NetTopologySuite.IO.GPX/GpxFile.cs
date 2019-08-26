using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A container that holds all the individual pieces of a GPX file and contains helper methods
    /// to (somwehat) easily read / write them on-the-fly.
    /// </summary>
    public sealed class GpxFile
    {
        private static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
        {
            CloseInput = false,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };

        private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings
        {
            CloseOutput = false,
            Indent = true,
            NewLineOnAttributes = true,
            OmitXmlDeclaration = true,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
        };

        private GpxMetadata _metadata = new GpxMetadata(creator: "NetTopologySuite.IO.GPX");

        /// <summary>
        /// Gets or sets the <see cref="GpxMetadata"/> instance that describes the contents of this
        /// GPX file.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown by the property setter, when trying to set the value to <see langword="null"/>.
        /// </exception>
        public GpxMetadata Metadata
        {
            get => _metadata;
            set => _metadata = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the list of <see cref="GpxWaypoint"/> instances that describe the top-level
        /// waypoints in this GPX file.
        /// </summary>
        public List<GpxWaypoint> Waypoints { get; } = new List<GpxWaypoint>();

        /// <summary>
        /// Gets the list of <see cref="GpxRoute"/> instances that describe the routes contained in
        /// this GPX file.
        /// </summary>
        public List<GpxRoute> Routes { get; } = new List<GpxRoute>();

        /// <summary>
        /// Gets the list of <see cref="GpxTrack"/> instances that describe the tracks contained in
        /// this GPX file.
        /// </summary>
        public List<GpxTrack> Tracks { get; } = new List<GpxTrack>();

        /// <summary>
        /// Gets or sets the top-level extensions for the entire file.
        /// </summary>
        public object Extensions { get; set; }

        /// <summary>
        /// Reads the XML representation of a GPX file, and returns the result as an instance of
        /// <see cref="GpxFile"/>.
        /// </summary>
        /// <param name="text">
        /// The string representation of the GPX file.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxReaderSettings"/> instance to use to control how GPX instances get
        /// read in, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <returns>
        /// A <see cref="GpxFile"/> instance that contains the same contents as the given GPX file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="text"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="XmlException">
        /// Thrown when <paramref name="text"/> does not specify a valid GPX file (i.e., cases
        /// where XSD schema validation would fail, and/or some values are <b>well</b> outside of
        /// the slightly stricter, but still completely reasonable, limits imposed by the idiomatic
        /// .NET data types above and beyond the XSD limits).
        /// </exception>
        public static GpxFile Parse(string text, GpxReaderSettings settings)
        {
            using (var stringReader = new StringReader(text ?? throw new ArgumentNullException(nameof(text))))
            using (var xmlReader = XmlReader.Create(stringReader, XmlReaderSettings))
            {
                return ReadFrom(xmlReader, settings);
            }
        }

        /// <summary>
        /// Reads the XML representation of a GPX file, and returns the result as an instance of
        /// <see cref="GpxFile"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> to read from.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxReaderSettings"/> instance to use to control how GPX instances get
        /// read in, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <returns>
        /// A <see cref="GpxFile"/> instance that contains the same contents as the given GPX file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="XmlException">
        /// Thrown when <paramref name="reader"/> does not specify a valid GPX file (i.e., cases
        /// where XSD schema validation would fail, and/or some values are <b>well</b> outside of
        /// the slightly stricter, but still completely reasonable, limits imposed by the idiomatic
        /// .NET data types above and beyond the XSD limits).
        /// </exception>
        public static GpxFile ReadFrom(XmlReader reader, GpxReaderSettings settings)
        {
            var result = new GpxFile();
            var visitor = new GpxFileBuilderVisitor(result);
            GpxReader.Read(reader, settings, visitor);
            return result;
        }

        /// <summary>
        /// Writes this file to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to write to.
        /// </param>
        /// <param name="settings">
        /// The <see cref="GpxWriterSettings"/> instance to use to control how GPX instances get
        /// written out, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public void WriteTo(XmlWriter writer, GpxWriterSettings settings) =>
            GpxWriter.Write(writer,
                            settings,
                            Metadata,
                            Waypoints,
                            Routes,
                            Tracks,
                            Extensions);

        /// <summary>
        /// Builds the string representation of this file.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="GpxWriterSettings"/> instance to use to control how GPX instances get
        /// written out, or <c>null</c> to use a general-purpose default.
        /// </param>
        /// <returns>
        /// The string representation of this file.
        /// </returns>
        public string BuildString(GpxWriterSettings settings)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlWriterSettings))
            {
                WriteTo(xmlWriter, settings);
            }

            return stringBuilder.ToString();
        }

        private sealed class GpxFileBuilderVisitor : GpxVisitorBase
        {
            private readonly GpxFile _fileToBuild;

            public GpxFileBuilderVisitor(GpxFile fileToBuild) => _fileToBuild = fileToBuild;

            public override void VisitMetadata(GpxMetadata metadata) => _fileToBuild.Metadata = metadata;

            public override void VisitWaypoint(GpxWaypoint waypoint) => _fileToBuild.Waypoints.Add(waypoint);

            public override void VisitRoute(GpxRoute route) => _fileToBuild.Routes.Add(route);

            public override void VisitTrack(GpxTrack track) => _fileToBuild.Tracks.Add(track);

            public override void VisitExtensions(object extensions) => _fileToBuild.Extensions = extensions;
        }
    }
}
