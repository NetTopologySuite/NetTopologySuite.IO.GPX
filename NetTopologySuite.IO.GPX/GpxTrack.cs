using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxTrack
    {
        public GpxTrack(string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, uint? number, string classification, ImmutableArray<GpxTrackSegment> segments, object extensions)
        {
            this.Name = name;
            this.Comment = comment;
            this.Description = description;
            this.Source = source;
            this.Links = links;
            this.Number = number;
            this.Classification = classification;
            this.Segments = segments;
            this.Extensions = extensions;
        }

        public string Name { get; }

        public string Comment { get; }

        public string Description { get; }

        public string Source { get; }

        public ImmutableArray<GpxWebLink> Links { get; }

        public uint? Number { get; }

        public string Classification { get; }

        public ImmutableArray<GpxTrackSegment> Segments { get; }

        public object Extensions { get; }

        public static GpxTrack Load(XElement element, GpxReaderSettings settings, Func<XElement, object> trackExtensionCallback, Func<XElement, object> trackSegmentExtensionCallback, Func<XElement, object> waypointExtensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (trackExtensionCallback is null)
            {
                throw new ArgumentNullException(nameof(trackExtensionCallback));
            }

            if (trackSegmentExtensionCallback is null)
            {
                throw new ArgumentNullException(nameof(trackSegmentExtensionCallback));
            }

            if (waypointExtensionCallback is null)
            {
                throw new ArgumentNullException(nameof(waypointExtensionCallback));
            }

            return new GpxTrack(
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                number: Helpers.ParseUInt32(element.GpxElement("number")?.Value),
                classification: element.GpxElement("type")?.Value,
                segments: ImmutableArray.CreateRange(element.GpxElements("trkseg").Select(el => GpxTrackSegment.LoadNoValidation(el, settings, trackSegmentExtensionCallback, waypointExtensionCallback))),
                extensions: trackExtensionCallback(element.GpxElement("extensions")));
        }
    }
}
