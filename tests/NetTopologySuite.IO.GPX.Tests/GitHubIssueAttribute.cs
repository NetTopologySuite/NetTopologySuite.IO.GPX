using System;

using Xunit.Sdk;

namespace NetTopologySuite.IO
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [TraitDiscoverer("NetTopologySuite.IO.GitHubIssueTraitDiscoverer", "NetTopologySuite.IO.GPX.Tests")]
    public sealed class GitHubIssueAttribute : Attribute, ITraitAttribute
    {
        public GitHubIssueAttribute(params uint[] issueNumbers) { }
    }
}
