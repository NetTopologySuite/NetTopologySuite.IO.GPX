# NetTopologySuite.IO.GPX Release Notes

## [0.5.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/10)
- `GpxWriterSettings.TimeZoneInfo` is no longer completely ignored ([#30](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/30)).
- Fix an issue where written timestamps were being rounded to the nearest second instead of preserving as many significant digits as `DateTime` allows ([#31](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/31)).

## [0.4.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/7)
- `GpxReaderSettings` now has an `IgnoreBadDateTime` property to use for ignoring a timestamp value that we cannot parse as an instance of the `DateTime` struct, to work around a `0000-00-00T00:00:00Z` coming from CompeGPS ([#29](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/29)).
- Update referenced packages to latest revisions.

## [0.3.2](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/9)
- The opt-in `GpxReaderSettings.AllowMissingVersionAttribute` property added in 0.3.1 has been replaced by `IgnoreVersionAttribute`, which enables the same situations as `AllowMissingVersionAttribute` did, plus situations where `version` was specified as something other than `version='1.1'` ([#28](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/28)).

## [0.3.1](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/8)
- `GpxReaderSettings` now has an opt-in `AllowMissingVersionAttribute` property, to allow reading files without a `version` attribute ([#27](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/27)).

## [0.3.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/4)
- `GpxReaderSettings` now has a `DefaultCreatorIfMissing` property to use for filling in a missing `GpxMetadata.Creator` value, to help read values saved from legacy versions of IHM ([#23](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/23)).
- Custom `GpxExtensionReader` and `GpxExtensionWriter` subclasses can now override just one method if there's a common way to handle extensions regardless of where they show up ([#26](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/26)).
- All immutable data model types now override `Equals(object)` and `GetHashCode()` to have "value" semantics ([#9](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/9)).
    - Mutable types, such as `GpxFile`, do **not** do this, for your own safety.
    - In **all** lists, including lists of web links, ordering matters.  [A, B] is not considered equal to [B, A].
    - `Extensions` values **are** considered for equality.
        - If you do not use a custom `GpxExtensionReader` for extensions, or if yours always returns `ImmutableXElementContainer`, then this should work like you would expect it to work.
        - Otherwise, whenever you return non-`null`, it is your responsibility to make sure that the result's `Equals(object)` and `GetHashCode()` semantics match what you want the container's semantics to be.
- `GpxWebLink` now has a constructor that accepts just the value for `Href`, since that's the only required value.
- `GpxLongitude` no longer permits +180 as a legal value, in keeping with the GPX 1.1 schema. ([#25](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/25)).
    - Converting from NTS features will automatically replace +180 with the equivalent -180.
- `GpxLongitude`, `GpxLatitude`, `GpxDegrees`, and `GpxDgpsStationId` now all have static `MinValue` and `MaxValue` fields to get the smallest and largest legal values.
- `GpxBounds` now has a shortcut for getting the a value that covers the entire WGS-84 ellipsoid.
- `GpxWaypoint` now has a constructor that accepts a `GeoAPI.Geometries.Coordinate` instance, for convenience ([#20](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/20)).
- `GpxWaypoint` constructors and helper method now both reject infinite values of `elevationInMeters` ([#24](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/24)).
- `GpxWaypoint` constructors and helper methods now throw `ArgumentOutOfRangeException` instead of `ArgumentException` in situations where the former is more appropriate.
- Trying to set `GpxFile.Metadata` to `null` now throws `ArgumentNullException` right away ([#21](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/21)).
- Add methods on `GpxFile` to allow converting to / from string, for convenience ([#19](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/19)).

## [0.2.1](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/5)
- Add a default constructor for `GpxTrackSegment` so that its `.WithX` methods are actually meaningful ([#22](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/22)).

## [0.2.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/2)
- Give most data objects constructors and `.WithX` methods to make it easier to create instances that only set a few members ([#13](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/13)).
- Add `GpxFile` as a significantly easier way to read + write our underlying data object representation all in one go ([#12](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/12) / [#14](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/14)).
- Make `NetTopologySuiteFeatureBuilderGpxVisitor` internal, since it's really just an implementation detail.
- Some more appropriate early `null` handling in miscellaneous places across-the-board.
- Default time zone is now UTC instead of local, since the documentation indicates that this is the convention.
- Reorder some data members (both in constructors and `.ToString()` overrides) according to XSD order.
- Remove the awkward `GpxReader.ReadFeatures` overload that took in `NetTopologySuiteFeatureBuilderGpxVisitor`.
- Fix an off-by-one issue that would cause features to get skipped over when not separated by whitespace. ([#18](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/18)).
- Fix an issue where values very close to zero would be written in scientific notation, violating the GPX spec. ([#15](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/15)).
- Add some more `ToString()` overrides
- `GpxWaypoint` constructor parameters reordered to match the order of the elements that the XSD schema requires
- `ImmutableGpxWaypointTable` now throws `ArgumentException` if it encounters a `null` waypoint.
- Rename `GpxMetadata.CreationTime` to `GpxMetadata.CreationTimeUtc` and validate that it is, in fact, marked as UTC (when specified at all).
- xmldoc ([#6](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/6))
- No longer exposing save / load methods on individual data elements.

## [0.1.1](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/3)
- Fix a copy-paste error in `GpxTrack` ([#11](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/11))

## 0.1.0
- Initial release.
