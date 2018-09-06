using System;

using Xunit.Sdk;

namespace NetTopologySuite.IO
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [TraitDiscoverer("NetTopologySuite.IO.RegressionTraitDiscoverer", "NetTopologySuite.IO.GPX.Tests")]
    public sealed class RegressionAttribute : Attribute, ITraitAttribute
    {
    }
}
