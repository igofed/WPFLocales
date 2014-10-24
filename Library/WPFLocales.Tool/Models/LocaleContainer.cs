using System.IO;
using System.Xml.Serialization;
using WpfLocales.Model.Xml;

namespace WPFLocales.Tool.Models
{
    internal class LocaleContainer
    {
        public XmlLocale Locale { get; set; }
        public string Path { get; set; }


        public static LocaleContainer ReadFromFile(string path)
        {
            if (System.IO.Path.GetExtension(path) != ".locale")
                throw new FileFormatException("Locale file should have .locale extension");
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format(@"Locale file didn't found at ""{0}""", path));

            XmlLocale locale;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(XmlLocale));
                locale = (XmlLocale)serializer.Deserialize(stream);
            }

            return new LocaleContainer { Locale = locale, Path = path };
        }

        public static void WriteToFile(LocaleContainer locale)
        {
            using (var stream = new FileStream(locale.Path, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(XmlLocale));
                serializer.Serialize(stream, locale.Locale);
            }
        }
    }
}
