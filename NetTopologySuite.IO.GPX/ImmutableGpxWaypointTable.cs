using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    // This could implement the slightly nicer IImmutableList<GpxWaypoint>, but I just don't feel
    // like adding all the mutation operations.  It's totally immutable, though: after creation, no
    // member fields can be overwritten, and none of their contents may be altered, without going to
    // unsafe code.
    public sealed class ImmutableGpxWaypointTable : IReadOnlyList<GpxWaypoint>
    {
        private readonly ImmutableArray<GpxLongitude> longitudes = ImmutableArray<GpxLongitude>.Empty;

        private readonly ImmutableArray<GpxLatitude> latitudes = ImmutableArray<GpxLatitude>.Empty;

        private readonly OptionalStructList<double> elevationsInMeters;

        private readonly OptionalStructList<DateTime> timestampsUtc;

        private readonly OptionalClassList<string> names;

        private readonly OptionalClassList<string> descriptions;

        private readonly OptionalClassList<string> symbolTexts;

        private readonly OptionalStructList<GpxDegrees> magneticVariations;

        private readonly OptionalStructList<double> geoidHeights;

        private readonly OptionalClassList<string> comments;

        private readonly OptionalClassList<string> sources;

        private readonly OptionalImmutableArrayList<GpxWebLink> webLinkLists;

        private readonly OptionalClassList<string> classifications;

        private readonly OptionalStructList<GpxFixKind> fixKinds;

        private readonly OptionalStructList<uint> numbersOfSatellites;

        private readonly OptionalStructList<double> horizontalDilutionsOfPrecision;

        private readonly OptionalStructList<double> verticalDilutionsOfPrecision;

        private readonly OptionalStructList<double> positionDilutionsOfPrecision;

        private readonly OptionalStructList<double> secondsSinceLastDgpsUpdates;

        private readonly OptionalStructList<GpxDgpsStationId> dgpsStationIds;

        private readonly OptionalClassList<object> allExtensions;

        internal ImmutableGpxWaypointTable(IEnumerable<XElement> elements, GpxReaderSettings settings, Func<XElement, object> extensionCallback)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (extensionCallback is null)
            {
                throw new ArgumentNullException(nameof(extensionCallback));
            }

            int cnt = 0;
            var longitudes = new List<GpxLongitude>();
            var latitudes = new List<GpxLatitude>();
            List<double?> elevationsInMeters = null;
            List<DateTime?> timestampsUtc = null;
            List<string> names = null;
            List<string> descriptions = null;
            List<string> symbolTexts = null;
            List<GpxDegrees?> magneticVariations = null;
            List<double?> geoidHeights = null;
            List<string> comments = null;
            List<string> sources = null;
            List<ImmutableArray<GpxWebLink>> webLinkLists = null;
            List<string> classifications = null;
            List<GpxFixKind?> fixKinds = null;
            List<uint?> numbersOfSatellites = null;
            List<double?> horizontalDilutionsOfPrecision = null;
            List<double?> verticalDilutionsOfPrecision = null;
            List<double?> positionDilutionsOfPrecision = null;
            List<double?> secondsSinceLastDgpsUpdates = null;
            List<GpxDgpsStationId?> dgpsStationIds = null;
            List<object> allExtensions = null;

            foreach (var element in elements)
            {
                longitudes.Add(Helpers.ParseLongitude(element.GpxAttribute("lon")?.Value) ?? throw new XmlException("waypoint must have lon attribute"));
                latitudes.Add(Helpers.ParseLatitude(element.GpxAttribute("lat")?.Value) ?? throw new XmlException("waypoint must have lat attribute"));
                Add(ref elevationsInMeters, Helpers.ParseDouble(element.GpxElement("ele")?.Value), cnt);
                Add(ref timestampsUtc, Helpers.ParseDateTimeUtc(element.GpxElement("time")?.Value, settings.TimeZoneInfo), cnt);
                Add(ref names, element.GpxElement("name")?.Value, cnt);
                Add(ref descriptions, element.GpxElement("desc")?.Value, cnt);
                Add(ref symbolTexts, element.GpxElement("sym")?.Value, cnt);
                Add(ref magneticVariations, Helpers.ParseDegrees(element.GpxElement("magvar")?.Value), cnt);
                Add(ref geoidHeights, Helpers.ParseDouble(element.GpxElement("geoidheight")?.Value), cnt);
                Add(ref comments, element.GpxElement("cmt")?.Value, cnt);
                Add(ref sources, element.GpxElement("src")?.Value, cnt);
                Add(ref webLinkLists, ImmutableArray.CreateRange(element.GpxElements("link").Select(GpxWebLink.Load)), cnt);
                Add(ref classifications, element.GpxElement("type")?.Value, cnt);
                Add(ref fixKinds, Helpers.ParseFixKind(element.GpxElement("fix")?.Value), cnt);
                Add(ref numbersOfSatellites, Helpers.ParseUInt32(element.GpxElement("sat")?.Value), cnt);
                Add(ref horizontalDilutionsOfPrecision, Helpers.ParseDouble(element.GpxElement("hdop")?.Value), cnt);
                Add(ref verticalDilutionsOfPrecision, Helpers.ParseDouble(element.GpxElement("vdop")?.Value), cnt);
                Add(ref positionDilutionsOfPrecision, Helpers.ParseDouble(element.GpxElement("pdop")?.Value), cnt);
                Add(ref secondsSinceLastDgpsUpdates, Helpers.ParseDouble(element.GpxElement("ageofdgpsdata")?.Value), cnt);
                Add(ref dgpsStationIds, Helpers.ParseDgpsStationId(element.GpxElement("dgpsid")?.Value), cnt);
                Add(ref allExtensions, extensionCallback(element.GpxElement("extensions")), cnt);

                ++cnt;
            }

            this.longitudes = longitudes.ToImmutableArray();
            this.latitudes = latitudes.ToImmutableArray();
            this.elevationsInMeters = Optional(elevationsInMeters);
            this.timestampsUtc = Optional(timestampsUtc);
            this.names = Optional(names);
            this.descriptions = Optional(descriptions);
            this.symbolTexts = Optional(symbolTexts);
            this.magneticVariations = Optional(magneticVariations);
            this.geoidHeights = Optional(geoidHeights);
            this.comments = Optional(comments);
            this.sources = Optional(sources);
            this.webLinkLists = Optional(webLinkLists);
            this.classifications = Optional(classifications);
            this.fixKinds = Optional(fixKinds);
            this.numbersOfSatellites = Optional(numbersOfSatellites);
            this.horizontalDilutionsOfPrecision = Optional(horizontalDilutionsOfPrecision);
            this.verticalDilutionsOfPrecision = Optional(verticalDilutionsOfPrecision);
            this.positionDilutionsOfPrecision = Optional(positionDilutionsOfPrecision);
            this.secondsSinceLastDgpsUpdates = Optional(secondsSinceLastDgpsUpdates);
            this.dgpsStationIds = Optional(dgpsStationIds);
            this.allExtensions = Optional(allExtensions);

            this.Count = cnt;
        }

        public GpxWaypoint this[int index] => new GpxWaypoint(
            this.longitudes[index],
            this.latitudes[index],
            this.elevationsInMeters[index],
            this.timestampsUtc[index],
            this.names[index],
            this.descriptions[index],
            this.symbolTexts[index],
            this.magneticVariations[index],
            this.geoidHeights[index],
            this.comments[index],
            this.sources[index],
            this.webLinkLists[index],
            this.classifications[index],
            this.fixKinds[index],
            this.numbersOfSatellites[index],
            this.horizontalDilutionsOfPrecision[index],
            this.verticalDilutionsOfPrecision[index],
            this.positionDilutionsOfPrecision[index],
            this.secondsSinceLastDgpsUpdates[index],
            this.dgpsStationIds[index],
            this.allExtensions[index]);

        public int Count { get; }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<GpxWaypoint> IEnumerable<GpxWaypoint>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        private static void Add<T>(ref List<ImmutableArray<T>> lst, ImmutableArray<T> value, int cnt)
        {
            if (lst is null && !value.IsDefaultOrEmpty)
            {
                lst = new List<ImmutableArray<T>>();
                lst.AddRange(Enumerable.Repeat(ImmutableArray<T>.Empty, cnt));
            }

            lst?.Add(value);
        }

        private static void Add<T>(ref List<T> lst, T value, int cnt)
            where T : class
        {
            if (lst is null && !(value is null))
            {
                lst = new List<T>();
                lst.AddRange(Enumerable.Repeat(default(T), cnt));
            }

            lst?.Add(value);
        }

        private static void Add<T>(ref List<T?> lst, T? value, int cnt)
            where T : struct
        {
            if (lst is null && !(value is null))
            {
                lst = new List<T?>();
                lst.AddRange(Enumerable.Repeat(default(T?), cnt));
            }

            lst?.Add(value);
        }

        private static OptionalImmutableArrayList<T> Optional<T>(List<ImmutableArray<T>> values) => new OptionalImmutableArrayList<T>(values);

        private static OptionalClassList<T> Optional<T>(List<T> values) where T : class => new OptionalClassList<T>(values);

        private static OptionalStructList<T> Optional<T>(List<T?> values) where T : struct => new OptionalStructList<T>(values);

        public struct Enumerator : IEnumerator<GpxWaypoint>
        {
#pragma warning disable IDE0044 // Add readonly modifier
            private ImmutableGpxWaypointTable table;
#pragma warning restore IDE0044 // Add readonly modifier

            private int curr;

            internal Enumerator(ImmutableGpxWaypointTable table)
            {
                this.table = table;
                this.curr = -1;
            }

            public GpxWaypoint Current => this.table[this.curr];

            object IEnumerator.Current => this.Current;

            public bool MoveNext() => this.curr != this.table.Count &&
                                      ++this.curr != this.table.Count;

            void IDisposable.Dispose() { }

            void IEnumerator.Reset() { }
        }

        private readonly struct OptionalImmutableArrayList<T>
        {
            private readonly ImmutableArray<ImmutableArray<T>> values;

            public OptionalImmutableArrayList(List<ImmutableArray<T>> values) => this.values = values?.ToImmutableArray() ?? default;

            public ImmutableArray<T> this[int index] => this.values.IsDefault ? ImmutableArray<T>.Empty : this.values[index];
        }

        private readonly struct OptionalClassList<T>
            where T : class
        {
            private readonly ImmutableArray<T> values;

            public OptionalClassList(List<T> values) => this.values = values?.ToImmutableArray() ?? default;

            public T this[int index] => this.values.IsDefault ? null : this.values[index];
        }

        private readonly struct OptionalStructList<T>
            where T : struct
        {
            // the resolution of dotnet/corefx#11861 means we're probably not going to be getting
            // ImmutableBitArray, but we still should separate out HasValue so it packs more nicely.
            private readonly ImmutableArray<bool> flags;

            private readonly ImmutableArray<T> values;

            public OptionalStructList(List<T?> values)
            {
                if (values == null)
                {
                    this.flags = default;
                    this.values = default;
                }
                else
                {
                    var flagsBuilder = ImmutableArray.CreateBuilder<bool>(values.Count);
                    flagsBuilder.Count = values.Count;
                    var valuesBuilder = ImmutableArray.CreateBuilder<T>(values.Count);
                    valuesBuilder.Count = values.Count;
                    int i = 0;
                    foreach (var value in values)
                    {
                        if (value.HasValue)
                        {
                            flagsBuilder[i] = true;
                            valuesBuilder[i] = value.GetValueOrDefault();
                        }

                        i++;
                    }

                    this.flags = flagsBuilder.MoveToImmutable();
                    this.values = valuesBuilder.MoveToImmutable();
                }
            }

            public T? this[int index] => (this.flags.IsDefault || !this.flags[index]) ? this.values[index] : default(T?);
        }
    }
}
