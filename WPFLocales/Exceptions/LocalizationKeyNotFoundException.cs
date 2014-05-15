using System;

namespace WPFLocales.Exceptions
{
    public class LocalizationKeyNotFoundException : Exception
    {
        public LocalizationKeyNotFoundException() { }
        public LocalizationKeyNotFoundException(string message) : base(message) { }
    }
}
