using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WPFLocales.Model;

namespace WPFLocales.Xml
{
    [XmlRoot("Locale")]
    public class XmlLocale : ILocale
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlElement("Group")]
        public List<XmlLocaleGroup> Groups { get; set; }

        List<ILocaleGroup> ILocale.Groups
        {
            get
            {
                return Groups.Cast<ILocaleGroup>().ToList();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
