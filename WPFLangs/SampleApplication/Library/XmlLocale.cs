using System.Collections.Generic;
using System.Xml.Serialization;

namespace SampleApplication.Library
{
    [XmlRoot("Locale")]
    public class XmlLocale
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlElement("Group")]
        public List<XmlLocaleGroup> Groups { get; set; }
    }
}
