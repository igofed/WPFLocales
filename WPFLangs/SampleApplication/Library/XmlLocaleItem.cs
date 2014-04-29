using System.Xml.Serialization;

namespace SampleApplication.Library
{
    public class XmlLocaleItem
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlText]
        public string Value { get; set; }
        [XmlAttribute]
        public string Comment { get; set; }
    }
}
