using System.IO;
using Microsoft.Win32;
using WpfLocales.Model.Xml;

namespace WPFLocales.Tool.Models
{
    internal class FileDialogUtils
    {
        public static string FindLocaleFile()
        {
            var dialog = new OpenFileDialog { Filter = "Locale files|*.locale" };
            var dialogResult = dialog.ShowDialog();
            return dialogResult.Value ? dialog.FileName : null;
        }

        public static string CreateLocaleFile()
        {
            var dialog = new SaveFileDialog { Filter = "Locale files|*.locale" };
            var dialogResult = dialog.ShowDialog();
            if (dialogResult != true)
                return null;

            var fileName = dialog.FileName;
            var locale = new XmlLocale { Key = Path.GetFileNameWithoutExtension(fileName) };
            var localeContainer = new LocaleContainer { Locale = locale, Path = fileName };
            LocaleContainer.WriteToFile(localeContainer);
            
            return fileName;
        }
    }
}
