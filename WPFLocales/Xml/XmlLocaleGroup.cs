using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WPFLocales.Model;

namespace WPFLocales.Xml
{
    public class XmlLocaleGroup : ILocaleGroup
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlElement("Item")]
        public List<XmlLocaleItem> Items { get; set; }

        List<ILocaleItem> ILocaleGroup.Items
        {
            get
            {
                return Items.Cast<ILocaleItem>().ToList();
            }
        }
    }
}
