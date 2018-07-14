using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxRoute
    {
        public GpxRoute(string name, string comment, string description, string source, ImmutableArray<GpxWebLink> links, uint? number, string classification, ImmutableGpxWaypointTable waypoints, object extensions)
        {
            this.Name = name;
            this.Comment = comment;
            this.Description = description;
            this.Source = source;
            this.Links = links;
            this.Number = number;
            this.Classification = classification;
            this.Waypoints = waypoints;
            this.Extensions = extensions;
        }

        public string Name { get; }

        public string Comment { get; }

        public string Description { get; }

        public string Source { get; }

        public ImmutableArray<GpxWebLink> Links { get; }

        public uint? Number { get; }

        public string Classification { get; }

        public ImmutableGpxWaypointTable Waypoints { get; }

        public object Extensions { get; }

        public static GpxRoute Load(XElement element, GpxReaderSettings settings, Func<XElement, object> routeExtensionCallback, Func<XElement, object> waypointExtensionCallback)
        {
            if (element is null)
            {
                return null;
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (routeExtensionCallback is null)
            {
                throw new ArgumentNullException(nameof(routeExtensionCallback));
            }

            if (waypointExtensionCallback is null)
            {
                throw new ArgumentNullException(nameof(waypointExtensionCallback));
            }

            return new GpxRoute(
                name: element.GpxElement("name")?.Value,
                comment: element.GpxElement("cmt")?.Value,
                description: element.GpxElement("desc")?.Value,
                source: element.GpxElement("src")?.Value,
                links: ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)),
                number: Helpers.ParseUInt32(element.GpxElement("number")?.Value),
                classification: element.GpxElement("type")?.Value,
                waypoints: new ImmutableGpxWaypointTable(element.GpxElements("rtept"), settings, waypointExtensionCallback),
                extensions: routeExtensionCallback(element.GpxElement("extensions")));
        }
    }
}
