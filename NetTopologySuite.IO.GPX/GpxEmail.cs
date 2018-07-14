using System.Xml;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    public sealed class GpxEmail
    {
        public GpxEmail(string id, string domain)
        {
            this.Id = id;
            this.Domain = domain;
        }

        public string Id { get; }

        public string Domain { get; }

        public static GpxEmail Load(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            return new GpxEmail(
                id: element.GpxAttribute("id")?.Value ?? throw new XmlException("email element must have both 'id' and 'domain' attributes"),
                domain: element.GpxAttribute("domain")?.Value ?? throw new XmlException("email element must have both 'id' and 'domain' attributes"));
        }

        public override string ToString() => this.Id + "@" + this.Domain;
    }
}
