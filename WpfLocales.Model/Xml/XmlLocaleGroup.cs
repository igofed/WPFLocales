using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WPFLocales.Model;

namespace WpfLocales.Model.Xml
{
    public class XmlLocaleGroup : ILocaleGroup
    {
        [XmlAttribute]
        public string Key { get; set; }
       
        [XmlAttribute]
        public string Comment { get; set; }

        [XmlElement("Item")]
        public List<XmlLocaleItem> Items { get; set; }

        IList<ILocaleItem> ILocaleGroup.Items
        {
            get
            {
                return Items.Cast<ILocaleItem>().ToList();
            }
        }
    }
}
