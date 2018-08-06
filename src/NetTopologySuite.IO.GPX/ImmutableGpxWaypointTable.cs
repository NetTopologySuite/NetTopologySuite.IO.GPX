using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public ImmutableGpxWaypointTable(IEnumerable<XElement> elements, GpxReaderSettings settings, Func<IEnumerable<XElement>, object> extensionCallback)
            : this(elements is null ? throw new ArgumentNullException(nameof(elements)) :
                   settings is null ? throw new ArgumentNullException(nameof(settings)) :
                   extensionCallback is null ? throw new ArgumentNullException(nameof(extensionCallback)) :
                   elements.Select(element => element is null ? throw new ArgumentException("No null elements are allowed", nameof(elements)) : GpxWaypoint.Load(element, settings, extensionCallback)))
        {
        }

        public ImmutableGpxWaypointTable(IEnumerable<GpxWaypoint> waypoints)
        {
            switch (waypoints)
            {
                case null:
                    throw new ArgumentNullException(nameof(waypoints));

                case ImmutableGpxWaypointTable otherTable:
                    this.longitudes = otherTable.longitudes;
                    this.latitudes = otherTable.latitudes;
                    this.elevationsInMeters = otherTable.elevationsInMeters;
                    this.timestampsUtc = otherTable.timestampsUtc;
                    this.names = otherTable.names;
                    this.descriptions = otherTable.descriptions;
                    this.symbolTexts = otherTable.symbolTexts;
                    this.magneticVariations = otherTable.magneticVariations;
                    this.geoidHeights = otherTable.geoidHeights;
                    this.comments = otherTable.comments;
                    this.sources = otherTable.sources;
                    this.webLinkLists = otherTable.webLinkLists;
                    this.classifications = otherTable.classifications;
                    this.fixKinds = otherTable.fixKinds;
                    this.numbersOfSatellites = otherTable.numbersOfSatellites;
                    this.horizontalDilutionsOfPrecision = otherTable.horizontalDilutionsOfPrecision;
                    this.verticalDilutionsOfPrecision = otherTable.verticalDilutionsOfPrecision;
                    this.positionDilutionsOfPrecision = otherTable.positionDilutionsOfPrecision;
                    this.secondsSinceLastDgpsUpdates = otherTable.secondsSinceLastDgpsUpdates;
                    this.dgpsStationIds = otherTable.dgpsStationIds;
                    this.allExtensions = otherTable.allExtensions;
                    this.Count = otherTable.Count;
                    return;
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

            foreach (var waypoint in waypoints)
            {
                if (waypoint is null)
                {
                    throw new ArgumentException("No null waypoints are allowed", nameof(waypoints));
                }

                longitudes.Add(waypoint.Longitude);
                latitudes.Add(waypoint.Latitude);
                Add(ref elevationsInMeters, waypoint.ElevationInMeters, cnt);
                Add(ref timestampsUtc, waypoint.TimestampUtc, cnt);
                Add(ref names, waypoint.Name, cnt);
                Add(ref descriptions, waypoint.Description, cnt);
                Add(ref symbolTexts, waypoint.SymbolText, cnt);
                Add(ref magneticVariations, waypoint.MagneticVariation, cnt);
                Add(ref geoidHeights, waypoint.GeoidHeight, cnt);
                Add(ref comments, waypoint.Comment, cnt);
                Add(ref sources, waypoint.Source, cnt);
                Add(ref webLinkLists, waypoint.Links, cnt);
                Add(ref classifications, waypoint.Classification, cnt);
                Add(ref fixKinds, waypoint.FixKind, cnt);
                Add(ref numbersOfSatellites, waypoint.NumberOfSatellites, cnt);
                Add(ref horizontalDilutionsOfPrecision, waypoint.HorizontalDilutionOfPrecision, cnt);
                Add(ref verticalDilutionsOfPrecision, waypoint.VerticalDilutionOfPrecision, cnt);
                Add(ref positionDilutionsOfPrecision, waypoint.PositionDilutionOfPrecision, cnt);
                Add(ref secondsSinceLastDgpsUpdates, waypoint.SecondsSinceLastDgpsUpdate, cnt);
                Add(ref dgpsStationIds, waypoint.DgpsStationId, cnt);
                Add(ref allExtensions, waypoint.Extensions, cnt);

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

        /// <inheritdoc />
        public GpxWaypoint this[int index] => new GpxWaypoint(
            this.longitudes[index],
            this.latitudes[index],
            this.elevationsInMeters[index],
            this.timestampsUtc[index],
            this.magneticVariations[index],
            this.geoidHeights[index],
            this.names[index],
            this.comments[index],
            this.descriptions[index],
            this.sources[index],
            this.webLinkLists[index],
            this.symbolTexts[index],
            this.classifications[index],
            this.fixKinds[index],
            this.numbersOfSatellites[index],
            this.horizontalDilutionsOfPrecision[index],
            this.verticalDilutionsOfPrecision[index],
            this.positionDilutionsOfPrecision[index],
            this.secondsSinceLastDgpsUpdates[index],
            this.dgpsStationIds[index],
            this.allExtensions[index]);

        /// <inheritdoc />
        public int Count { get; }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator<GpxWaypoint> IEnumerable<GpxWaypoint>.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        public override string ToString() => Helpers.BuildString((nameof(this.Count), this.Count));

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

        /// <inheritdoc />
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

            /// <inheritdoc />
            public GpxWaypoint Current => this.table[this.curr];

            /// <inheritdoc />
            object IEnumerator.Current => this.Current;

            /// <inheritdoc />
            public bool MoveNext() => this.curr != this.table.Count &&
                                      ++this.curr != this.table.Count;

            /// <inheritdoc />
            void IDisposable.Dispose() { }

            /// <inheritdoc />
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

            public T? this[int index] => (this.flags.IsDefault || !this.flags[index]) ? default(T?) : this.values[index];
        }
    }
}
