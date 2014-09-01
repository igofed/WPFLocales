using System;

namespace WPFLocales
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LocalizationAssembly : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class LocalizationDirectory : Attribute
    {
        public string Value { get; private set; }
        
        public LocalizationDirectory(string localizationDirectory)
        {
            Value = localizationDirectory;
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class LocalesDirectory : Attribute
    {
        public string Value { get; private set; }

        public LocalesDirectory(string localesDirectory)
        {
            Value = localesDirectory;
        }
    }
}
