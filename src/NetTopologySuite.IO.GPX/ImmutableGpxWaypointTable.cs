using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// An immutable list of <see cref="GpxWaypoint"/> instances.
    /// </summary>
    /// <remarks>
    /// Storage is optimized to minimize the cost of storing data members that are not actually used
    /// by any included waypoints.  For example, if <see cref="GpxWaypoint.Links"/> is empty on all
    /// included waypoints, then this table will not spend any memory on storing those empty lists.
    /// </remarks>
    public sealed class ImmutableGpxWaypointTable : IReadOnlyList<GpxWaypoint>
    {
        /// <summary>
        /// An empty instance of <see cref="ImmutableGpxWaypointTable"/>.
        /// </summary>
        public static readonly ImmutableGpxWaypointTable Empty = new ImmutableGpxWaypointTable(Enumerable.Empty<GpxWaypoint>());

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

        private readonly OptionalStructList<int> fixKinds;

        private readonly OptionalStructList<uint> numbersOfSatellites;

        private readonly OptionalStructList<double> horizontalDilutionsOfPrecision;

        private readonly OptionalStructList<double> verticalDilutionsOfPrecision;

        private readonly OptionalStructList<double> positionDilutionsOfPrecision;

        private readonly OptionalStructList<double> secondsSinceLastDgpsUpdates;

        private readonly OptionalStructList<GpxDgpsStationId> dgpsStationIds;

        private readonly OptionalClassList<object> allExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableGpxWaypointTable"/> class.
        /// </summary>
        /// <param name="waypoints">
        /// The <see cref="GpxWaypoint"/> instances to store.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="waypoints"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when an element of <paramref name="waypoints"/> is <see langword="null" />
        /// </exception>
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
            List<int?> fixKinds = null;
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
                Add(ref fixKinds, (int?)waypoint.FixKind, cnt);
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

        internal ImmutableGpxWaypointTable(IEnumerable<XElement> elements, GpxReaderSettings settings, Func<IEnumerable<XElement>, object> extensionCallback)
            : this(elements is null ? throw new ArgumentNullException(nameof(elements)) :
                   settings is null ? throw new ArgumentNullException(nameof(settings)) :
                   extensionCallback is null ? throw new ArgumentNullException(nameof(extensionCallback)) :
                   elements.Select(element => element is null ? throw new ArgumentException("No null elements are allowed", nameof(elements)) : GpxWaypoint.Load(element, settings, extensionCallback)))
        {
        }

        /// <inheritdoc />
        public GpxWaypoint this[int index] => new GpxWaypoint(
            longitude: this.longitudes[index],
            latitude: this.latitudes[index],
            elevationInMeters: this.elevationsInMeters[index],
            timestampUtc: this.timestampsUtc[index],
            magneticVariation: this.magneticVariations[index],
            geoidHeight: this.geoidHeights[index],
            name: this.names[index],
            comment: this.comments[index],
            description: this.descriptions[index],
            source: this.sources[index],
            links: this.webLinkLists[index],
            symbolText: this.symbolTexts[index],
            classification: this.classifications[index],
            fixKind: (GpxFixKind?)this.fixKinds[index],
            numberOfSatellites: this.numbersOfSatellites[index],
            horizontalDilutionOfPrecision: this.horizontalDilutionsOfPrecision[index],
            verticalDilutionOfPrecision: this.verticalDilutionsOfPrecision[index],
            positionDilutionOfPrecision: this.positionDilutionsOfPrecision[index],
            secondsSinceLastDgpsUpdate: this.secondsSinceLastDgpsUpdates[index],
            dgpsStationId: this.dgpsStationIds[index],
            extensions: this.allExtensions[index]);

        /// <inheritdoc />
        public int Count { get; }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator<GpxWaypoint> IEnumerable<GpxWaypoint>.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is ImmutableGpxWaypointTable other) || this.Count != other.Count)
            {
                return false;
            }

            return this.longitudes.ListEquals(other.longitudes) &&
                   this.latitudes.ListEquals(other.latitudes) &&
                   this.elevationsInMeters.Equals(other.elevationsInMeters) &&
                   this.timestampsUtc.Equals(other.timestampsUtc) &&
                   this.magneticVariations.Equals(other.magneticVariations) &&
                   this.geoidHeights.Equals(other.geoidHeights) &&
                   this.names.Equals(other.names) &&
                   this.comments.Equals(other.comments) &&
                   this.descriptions.Equals(other.descriptions) &&
                   this.sources.Equals(other.sources) &&
                   this.webLinkLists.Equals(other.webLinkLists) &&
                   this.symbolTexts.Equals(other.symbolTexts) &&
                   this.classifications.Equals(other.classifications) &&
                   this.fixKinds.Equals(other.fixKinds) &&
                   this.numbersOfSatellites.Equals(other.numbersOfSatellites) &&
                   this.horizontalDilutionsOfPrecision.Equals(other.horizontalDilutionsOfPrecision) &&
                   this.verticalDilutionsOfPrecision.Equals(other.verticalDilutionsOfPrecision) &&
                   this.positionDilutionsOfPrecision.Equals(other.positionDilutionsOfPrecision) &&
                   this.secondsSinceLastDgpsUpdates.Equals(other.secondsSinceLastDgpsUpdates) &&
                   this.dgpsStationIds.Equals(other.dgpsStationIds) &&
                   this.allExtensions.Equals(other.allExtensions);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hc = 0;
            foreach (var waypoint in this)
            {
                hc = (hc, waypoint).GetHashCode();
            }

            return hc;
        }

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

        private static OptionalStructList<T> Optional<T>(List<T?> values) where T : unmanaged, IEquatable<T> => new OptionalStructList<T>(values);

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
            void IEnumerator.Reset() => this.curr = -1;
        }

        private readonly struct OptionalImmutableArrayList<T> : IEquatable<OptionalImmutableArrayList<T>>
        {
            private readonly ImmutableArray<ImmutableArray<T>> values;

            public OptionalImmutableArrayList(List<ImmutableArray<T>> values) => this.values = values?.ToImmutableArray() ?? default;

            public ImmutableArray<T> this[int index] => this.values.IsDefault ? ImmutableArray<T>.Empty : this.values[index];

            public override bool Equals(object obj) => obj is OptionalImmutableArrayList<T> other && this.Equals(other);

            public bool Equals(OptionalImmutableArrayList<T> other)
            {
                var selfValues = this.values;
                var otherValues = other.values;
                if (selfValues == otherValues)
                {
                    return true;
                }

                if (selfValues.IsDefault)
                {
                    return otherValues.IsDefault;
                }

                if (otherValues.IsDefault || selfValues.Length != otherValues.Length)
                {
                    return false;
                }

                for (int i = 0; i < selfValues.Length; i++)
                {
                    if (!selfValues[i].ListEquals(otherValues[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                var selfValues = this.values;
                if (selfValues.IsDefault)
                {
                    return 0;
                }

                int hc = 0;
                for (int i = 0; i < selfValues.Length; i++)
                {
                    hc = Helpers.HashHelpersCombine(hc, selfValues[i].ListToHashCode());
                }

                return hc;
            }
        }

        private readonly struct OptionalClassList<T> : IEquatable<OptionalClassList<T>>
            where T : class
        {
            private readonly ImmutableArray<T> values;

            public OptionalClassList(List<T> values) => this.values = values?.ToImmutableArray() ?? default;

            public T this[int index] => this.values.IsDefault ? null : this.values[index];

            public override bool Equals(object obj) => obj is OptionalClassList<T> other && this.Equals(other);

            public bool Equals(OptionalClassList<T> other) => this.values.ListEquals(other.values);

            public override int GetHashCode() => this.values.ListToHashCode();
        }

        private readonly struct OptionalStructList<T> : IEquatable<OptionalStructList<T>>
            where T : unmanaged, IEquatable<T>
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

            public override bool Equals(object obj) => obj is OptionalStructList<T> other && this.Equals(other);

            public bool Equals(OptionalStructList<T> other) => this.values.ListEquals(other.values) &&
                                                               this.flags.ListEquals(other.flags);

            public override int GetHashCode() => (this.flags.ListToHashCode(), this.values.ListToHashCode()).GetHashCode();
        }
    }
}
