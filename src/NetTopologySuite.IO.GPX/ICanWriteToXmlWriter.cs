using System.Xml;

namespace NetTopologySuite.IO
{
    internal interface ICanWriteToXmlWriter
    {
        void Save(XmlWriter writer);
    }
}
