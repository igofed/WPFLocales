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
        public LocalizationDirectory(string localizationDirectory)
        {
            
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class LocalesDirectory : Attribute
    {
        public LocalesDirectory(string localesDirectory)
        {
            
        }
    }
}
