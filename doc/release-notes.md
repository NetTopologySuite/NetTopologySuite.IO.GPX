# NetTopologySuite.IO.GPX Release Notes

## [0.2.0](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/2)
- Fix an issue where values very close to zero would be written in scientific notation, violating the GPX spec ([#15](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/11)).
- Added some more `ToString()` overrides
- `GpxWaypoint` constructor parameters reordered to match the order of the elements that the XSD schema requires
- `ImmutableGpxWaypointTable` now throws `ArgumentException` if it encounters a `null` waypoint.
- Rename `GpxMetadata.CreationTime` to `GpxMetadata.CreationTimeUtc` and validate that it is, in fact, marked as UTC (when specified at all).
- xmldoc ([#6](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/6))
- No longer exposing save / load methods on individual data elements.

## [0.1.1](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/milestone/3)
- Fix a copy-paste error in `GpxTrack` ([#11](https://github.com/NetTopologySuite/NetTopologySuite.IO.GPX/issues/11))

## 0.1.0
- Initial release.
