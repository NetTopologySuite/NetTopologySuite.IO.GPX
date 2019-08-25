using System.Collections.Generic;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace NetTopologySuite.IO
{
    public class RegressionTraitDiscoverer : TraitDiscoverer
    {
        public sealed override IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var result = new List<KeyValuePair<string, string>>();
            AddOtherTraits(traitAttribute, result);
            result.Add(KeyValuePair.Create("Regression", string.Empty));
            return result;
        }

        protected virtual void AddOtherTraits(IAttributeInfo traitAttribute, List<KeyValuePair<string, string>> result) { }
    }
}
