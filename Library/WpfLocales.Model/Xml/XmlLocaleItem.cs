using System.Xml.Serialization;
using WPFLocales.Model;

namespace WpfLocales.Model.Xml
{
    public class XmlLocaleItem : ILocaleItem
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlText]
        public string Value { get; set; }
        [XmlAttribute]
        public string Comment { get; set; }
    }
}
