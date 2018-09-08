using System;

using Xunit;

namespace NetTopologySuite.IO
{
    public sealed class GpxLongitudeTests
    {
        [Fact]
        [Regression]
        [GitHubIssue(25)]
        public void ConstructorShouldRejectPositive180()
        {
            Assert.Throws<ArgumentOutOfRangeException>("val", () => new GpxLongitude(180));
        }
    }
}
