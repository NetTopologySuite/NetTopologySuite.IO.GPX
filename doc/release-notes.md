# NetTopologySuite.IO.GPX Release Notes

## [0.3.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/4)
- Trying to set `GpxFile.Metadata` to `null` now throws `ArgumentNullException` right away ([#21](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/21)).
- Add methods on `GpxFile` to allow converting to / from string ([#19](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/19)).

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
