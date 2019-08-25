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

        private readonly ImmutableArray<GpxLongitude> _longitudes = ImmutableArray<GpxLongitude>.Empty;

        private readonly ImmutableArray<GpxLatitude> _latitudes = ImmutableArray<GpxLatitude>.Empty;

        private readonly OptionalStructList<double> _elevationsInMeters;

        private readonly OptionalStructList<DateTime> _timestampsUtc;

        private readonly OptionalClassList<string> _names;

        private readonly OptionalClassList<string> _descriptions;

        private readonly OptionalClassList<string> _symbolTexts;

        private readonly OptionalStructList<GpxDegrees> _magneticVariations;

        private readonly OptionalStructList<double> _geoidHeights;

        private readonly OptionalClassList<string> _comments;

        private readonly OptionalClassList<string> _sources;

        private readonly OptionalImmutableArrayList<GpxWebLink> _webLinkLists;

        private readonly OptionalClassList<string> _classifications;

        private readonly OptionalStructList<int> _fixKinds;

        private readonly OptionalStructList<uint> _numbersOfSatellites;

        private readonly OptionalStructList<double> _horizontalDilutionsOfPrecision;

        private readonly OptionalStructList<double> _verticalDilutionsOfPrecision;

        private readonly OptionalStructList<double> _positionDilutionsOfPrecision;

        private readonly OptionalStructList<double> _secondsSinceLastDgpsUpdates;

        private readonly OptionalStructList<GpxDgpsStationId> _dgpsStationIds;

        private readonly OptionalClassList<object> _allExtensions;

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
                    _longitudes = otherTable._longitudes;
                    _latitudes = otherTable._latitudes;
                    _elevationsInMeters = otherTable._elevationsInMeters;
                    _timestampsUtc = otherTable._timestampsUtc;
                    _names = otherTable._names;
                    _descriptions = otherTable._descriptions;
                    _symbolTexts = otherTable._symbolTexts;
                    _magneticVariations = otherTable._magneticVariations;
                    _geoidHeights = otherTable._geoidHeights;
                    _comments = otherTable._comments;
                    _sources = otherTable._sources;
                    _webLinkLists = otherTable._webLinkLists;
                    _classifications = otherTable._classifications;
                    _fixKinds = otherTable._fixKinds;
                    _numbersOfSatellites = otherTable._numbersOfSatellites;
                    _horizontalDilutionsOfPrecision = otherTable._horizontalDilutionsOfPrecision;
                    _verticalDilutionsOfPrecision = otherTable._verticalDilutionsOfPrecision;
                    _positionDilutionsOfPrecision = otherTable._positionDilutionsOfPrecision;
                    _secondsSinceLastDgpsUpdates = otherTable._secondsSinceLastDgpsUpdates;
                    _dgpsStationIds = otherTable._dgpsStationIds;
                    _allExtensions = otherTable._allExtensions;
                    Count = otherTable.Count;
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

            _longitudes = longitudes.ToImmutableArray();
            _latitudes = latitudes.ToImmutableArray();
            _elevationsInMeters = Optional(elevationsInMeters);
            _timestampsUtc = Optional(timestampsUtc);
            _names = Optional(names);
            _descriptions = Optional(descriptions);
            _symbolTexts = Optional(symbolTexts);
            _magneticVariations = Optional(magneticVariations);
            _geoidHeights = Optional(geoidHeights);
            _comments = Optional(comments);
            _sources = Optional(sources);
            _webLinkLists = Optional(webLinkLists);
            _classifications = Optional(classifications);
            _fixKinds = Optional(fixKinds);
            _numbersOfSatellites = Optional(numbersOfSatellites);
            _horizontalDilutionsOfPrecision = Optional(horizontalDilutionsOfPrecision);
            _verticalDilutionsOfPrecision = Optional(verticalDilutionsOfPrecision);
            _positionDilutionsOfPrecision = Optional(positionDilutionsOfPrecision);
            _secondsSinceLastDgpsUpdates = Optional(secondsSinceLastDgpsUpdates);
            _dgpsStationIds = Optional(dgpsStationIds);
            _allExtensions = Optional(allExtensions);

            Count = cnt;
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
            longitude: _longitudes[index],
            latitude: _latitudes[index],
            elevationInMeters: _elevationsInMeters[index],
            timestampUtc: _timestampsUtc[index],
            magneticVariation: _magneticVariations[index],
            geoidHeight: _geoidHeights[index],
            name: _names[index],
            comment: _comments[index],
            description: _descriptions[index],
            source: _sources[index],
            links: _webLinkLists[index],
            symbolText: _symbolTexts[index],
            classification: _classifications[index],
            fixKind: (GpxFixKind?)_fixKinds[index],
            numberOfSatellites: _numbersOfSatellites[index],
            horizontalDilutionOfPrecision: _horizontalDilutionsOfPrecision[index],
            verticalDilutionOfPrecision: _verticalDilutionsOfPrecision[index],
            positionDilutionOfPrecision: _positionDilutionsOfPrecision[index],
            secondsSinceLastDgpsUpdate: _secondsSinceLastDgpsUpdates[index],
            dgpsStationId: _dgpsStationIds[index],
            extensions: _allExtensions[index]);

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

            if (!(obj is ImmutableGpxWaypointTable other) || Count != other.Count)
            {
                return false;
            }

            return _longitudes.ListEquals(other._longitudes) &&
                   _latitudes.ListEquals(other._latitudes) &&
                   _elevationsInMeters.Equals(other._elevationsInMeters) &&
                   _timestampsUtc.Equals(other._timestampsUtc) &&
                   _magneticVariations.Equals(other._magneticVariations) &&
                   _geoidHeights.Equals(other._geoidHeights) &&
                   _names.Equals(other._names) &&
                   _comments.Equals(other._comments) &&
                   _descriptions.Equals(other._descriptions) &&
                   _sources.Equals(other._sources) &&
                   _webLinkLists.Equals(other._webLinkLists) &&
                   _symbolTexts.Equals(other._symbolTexts) &&
                   _classifications.Equals(other._classifications) &&
                   _fixKinds.Equals(other._fixKinds) &&
                   _numbersOfSatellites.Equals(other._numbersOfSatellites) &&
                   _horizontalDilutionsOfPrecision.Equals(other._horizontalDilutionsOfPrecision) &&
                   _verticalDilutionsOfPrecision.Equals(other._verticalDilutionsOfPrecision) &&
                   _positionDilutionsOfPrecision.Equals(other._positionDilutionsOfPrecision) &&
                   _secondsSinceLastDgpsUpdates.Equals(other._secondsSinceLastDgpsUpdates) &&
                   _dgpsStationIds.Equals(other._dgpsStationIds) &&
                   _allExtensions.Equals(other._allExtensions);
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
        public override string ToString() => Helpers.BuildString((nameof(Count), Count));

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
            private ImmutableGpxWaypointTable _table;
#pragma warning restore IDE0044 // Add readonly modifier

            private int curr;

            internal Enumerator(ImmutableGpxWaypointTable table)
            {
                _table = table;
                curr = -1;
            }

            /// <inheritdoc />
            public GpxWaypoint Current => _table[curr];

            /// <inheritdoc />
            object IEnumerator.Current => Current;

            /// <inheritdoc />
            public bool MoveNext() => curr != _table.Count &&
                                      ++curr != _table.Count;

            /// <inheritdoc />
            void IDisposable.Dispose() { }

            /// <inheritdoc />
            void IEnumerator.Reset() => curr = -1;
        }

        private readonly struct OptionalImmutableArrayList<T> : IEquatable<OptionalImmutableArrayList<T>>
        {
            private readonly ImmutableArray<ImmutableArray<T>> _values;

            public OptionalImmutableArrayList(List<ImmutableArray<T>> values) => _values = values?.ToImmutableArray() ?? default;

            public ImmutableArray<T> this[int index] => _values.IsDefault ? ImmutableArray<T>.Empty : _values[index];

            public override bool Equals(object obj) => obj is OptionalImmutableArrayList<T> other && Equals(other);

            public bool Equals(OptionalImmutableArrayList<T> other)
            {
                var selfValues = _values;
                var otherValues = other._values;
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
                var selfValues = _values;
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
            private readonly ImmutableArray<T> _values;

            public OptionalClassList(List<T> values) => _values = values?.ToImmutableArray() ?? default;

            public T this[int index] => _values.IsDefault ? null : _values[index];

            public override bool Equals(object obj) => obj is OptionalClassList<T> other && Equals(other);

            public bool Equals(OptionalClassList<T> other) => _values.ListEquals(other._values);

            public override int GetHashCode() => _values.ListToHashCode();
        }

        private readonly struct OptionalStructList<T> : IEquatable<OptionalStructList<T>>
            where T : unmanaged, IEquatable<T>
        {
            // the resolution of dotnet/corefx#11861 means we're probably not going to be getting
            // ImmutableBitArray, but we still should separate out HasValue so it packs more nicely.
            private readonly ImmutableArray<bool> _flags;

            private readonly ImmutableArray<T> _values;

            public OptionalStructList(List<T?> values)
            {
                if (values == null)
                {
                    _flags = default;
                    _values = default;
                }
                else
                {
                    int cnt = values.Count;

                    var flagsBuilder = ImmutableArray.CreateBuilder<bool>(cnt);
                    flagsBuilder.Count = cnt;
                    var valuesBuilder = ImmutableArray.CreateBuilder<T>(cnt);
                    valuesBuilder.Count = cnt;
                    for (int i = 0; i < cnt; i++)
                    {
                        var value = values[i];
                        if (value.HasValue)
                        {
                            flagsBuilder[i] = true;
                            valuesBuilder[i] = value.GetValueOrDefault();
                        }
                    }

                    _flags = flagsBuilder.MoveToImmutable();
                    _values = valuesBuilder.MoveToImmutable();
                }
            }

            public T? this[int index] => (_flags.IsDefault || !_flags[index]) ? default(T?) : _values[index];

            public override bool Equals(object obj) => obj is OptionalStructList<T> other && Equals(other);

            public bool Equals(OptionalStructList<T> other) => _values.ListEquals(other._values) &&
                                                               _flags.ListEquals(other._flags);

            public override int GetHashCode() => (_flags.ListToHashCode(), _values.ListToHashCode()).GetHashCode();
        }
    }
}
