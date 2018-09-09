using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    internal static class DataObjectBuilders
    {
        public static GpxMetadata RandomGpxMetadata(Random random)
        {
            return new GpxMetadata(
                creator: "Creator_" + random.Next(),
                name: "Name_" + random.Next(),
                description: "Description_" + random.Next(),
                author: new GpxPerson(
                    name: "Name_" + random.Next(),
                    email: new GpxEmail("nts", "example.com"),
                    link: RandomWebLink(random)),
                copyright: new GpxCopyright(
                     author: "Author_" + random.Next(),
                     year: random.Next(1990, 2100),
                     licenseUri: RandomUri(random)),
                links: ImmutableArray.CreateRange(
                    Enumerable.Repeat(0, random.Next(1, 5)).Select(_ => RandomWebLink(random))),
                creationTimeUtc: RandomDateTimeUtc(random),
                keywords: "Keywords_" + random.Next(),
                bounds: RandomBoundingBox(random),
                extensions: RandomExtensions(random));
        }

        public static GpxWaypoint RandomWaypoint(Random random)
        {
            return new GpxWaypoint(
                longitude: RandomGpxLongitude(random),
                latitude: RandomGpxLatitude(random),
                elevationInMeters: 5000 * random.NextDouble(),
                timestampUtc: RandomDateTimeUtc(random),
                magneticVariation: new GpxDegrees(360 * random.NextDouble()),
                geoidHeight: 5000 * random.NextDouble(),
                name: "Name_" + random.Next(),
                comment: "Comment_" + random.Next(),
                description: "Description_" + random.Next(),
                source: "Source_" + random.Next(),
                links: ImmutableArray.CreateRange(
                    Enumerable.Repeat(0, random.Next(1, 5)).Select(_ => RandomWebLink(random))),
                symbolText: "SymbolText_" + random.Next(),
                classification: "Classification_" + random.Next(),
                fixKind: (GpxFixKind)random.Next(5),
                numberOfSatellites: (uint)random.Next(100),
                horizontalDilutionOfPrecision: 20 * random.NextDouble() - 10,
                verticalDilutionOfPrecision: 20 * random.NextDouble() - 10,
                positionDilutionOfPrecision: 20 * random.NextDouble() - 10,
                secondsSinceLastDgpsUpdate: 300 * random.NextDouble(),
                dgpsStationId: new GpxDgpsStationId((ushort)(random.Next(GpxDgpsStationId.MaxValue.Value + 1))),
                extensions: RandomExtensions(random));
        }

        public static GpxRoute RandomRoute(Random random)
        {
            return new GpxRoute(
                name: "Name_" + random.Next(),
                comment: "Comment_" + random.Next(),
                description: "Description_" + random.Next(),
                source: "Source_" + random.Next(),
                links: ImmutableArray.CreateRange(
                    Enumerable.Repeat(0, random.Next(1, 5)).Select(_ => RandomWebLink(random))),
                number: (uint)random.Next(100000),
                classification: "Classification_" + random.Next(),
                extensions: RandomExtensions(random),
                waypoints: new ImmutableGpxWaypointTable(
                    Enumerable.Repeat(0, random.Next(10)).Select(_ => RandomWaypoint(random))));
        }

        public static GpxTrack RandomTrack(Random random)
        {
            return new GpxTrack(
                name: "Name_" + random.Next(),
                comment: "Comment_" + random.Next(),
                description: "Description_" + random.Next(),
                source: "Source_" + random.Next(),
                links: ImmutableArray.CreateRange(
                    Enumerable.Repeat(0, random.Next(1, 5)).Select(_ => RandomWebLink(random))),
                number: (uint)random.Next(100000),
                classification: "Classification_" + random.Next(),
                extensions: RandomExtensions(random),
                segments: ImmutableArray.CreateRange(
                    Enumerable.Repeat(0, random.Next(10)).Select(_ => RandomTrackSegment(random))));
        }

        public static GpxTrackSegment RandomTrackSegment(Random random)
        {
            return new GpxTrackSegment(
                waypoints: new ImmutableGpxWaypointTable(
                    Enumerable.Repeat(0, random.Next(10)).Select(_ => RandomWaypoint(random))),
                extensions: RandomExtensions(random));
        }

        public static GpxBoundingBox RandomBoundingBox(Random random)
        {
            var longitude1 = RandomGpxLongitude(random);
            var longitude2 = RandomGpxLongitude(random);
            var latitude1 = RandomGpxLatitude(random);
            var latitude2 = RandomGpxLatitude(random);

            return new GpxBoundingBox(
                longitude1 < longitude2 ? longitude1 : longitude2,
                latitude1 < latitude2 ? latitude1 : latitude2,
                longitude1 < longitude2 ? longitude2 : longitude1,
                latitude1 < latitude2 ? latitude2 : latitude1);
        }

        public static GpxLongitude RandomGpxLongitude(Random random)
        {
            return new GpxLongitude(360 * random.NextDouble() - 180);
        }

        public static GpxLatitude RandomGpxLatitude(Random random)
        {
            return new GpxLatitude(180 * random.NextDouble() - 90);
        }

        public static GpxWebLink RandomWebLink(Random random)
        {
            return new GpxWebLink(
                href: RandomUri(random),
                text: "Text_" + random.Next(),
                contentType: "Type_" + random.Next());
        }

        public static ImmutableXElementContainer RandomExtensions(Random random)
        {
            var mainResult = new XElement(XName.Get("parent" + random.Next(), "http://www.example.com/schema"));
            for (int i = 0, cnt = random.Next(10); i < cnt; i++)
            {
                mainResult.Add(new XElement($"something{i}", new XAttribute("data", random.Next(100))));
            }

            return new ImmutableXElementContainer(new[] { mainResult });
        }

        private static Uri RandomUri(Random random)
        {
            return new Uri("http://example.com/" + random.Next());
        }

        private static DateTime RandomDateTimeUtc(Random random)
        {
            return new DateTime(random.Next(1990, 2100), random.Next(1, 12), random.Next(1, 28), random.Next(23), random.Next(59), random.Next(59), DateTimeKind.Utc);
        }
    }
}
