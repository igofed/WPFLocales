using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using WPFLocales.Model;

namespace WpfLocales.Model.Xml
{
    [XmlRoot("Locale")]
    public class XmlLocale : ILocale
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute, DefaultValue(false)]
        public bool IsDefault { get; set; }

        [XmlElement("Group")]
        public List<XmlLocaleGroup> Groups { get; set; }

        IList<ILocaleGroup> ILocale.Groups
        {
            get
            {
                return Groups.Cast<ILocaleGroup>().ToList();
            }
        }
    }
}
