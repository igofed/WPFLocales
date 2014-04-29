using System.Collections.Generic;
using System.Xml.Serialization;

namespace SampleApplication.Library
{
    public class XmlLocaleGroup
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlElement("Item")]
        public List<XmlLocaleItem> Items { get; set; }
    }
}
