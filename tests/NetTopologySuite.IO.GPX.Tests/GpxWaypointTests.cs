using System;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxWaypointTests
    {
        [Theory]
        [Regression]
        [GitHubIssue(24)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void ConstructorShouldRequireFiniteElevationWhenSpecified(double notFiniteValue)
        {
            var wpt = new GpxWaypoint(default, default);
            Assert.Throws<ArgumentOutOfRangeException>("elevationInMeters", () => wpt.WithElevationInMeters(notFiniteValue));
        }
    }
}
