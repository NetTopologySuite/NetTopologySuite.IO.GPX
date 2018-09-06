using System.Collections.Generic;
using System.Linq;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace NetTopologySuite.IO
{
    public sealed class GitHubIssueTraitDiscoverer : TraitDiscoverer
    {
        public override IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach (uint issueNumber in (uint[])traitAttribute.GetConstructorArguments().Single())
            {
                result.Add(KeyValuePair.Create("GitHub Issue", $"{issueNumber}"));
            }

            return result;
        }
    }
}
