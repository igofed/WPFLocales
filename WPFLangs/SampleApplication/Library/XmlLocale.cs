using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SampleApplication.Library
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
