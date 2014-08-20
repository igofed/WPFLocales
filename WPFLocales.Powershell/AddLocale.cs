﻿using System.Management.Automation;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsCommon.Add, "Locale")]
    public class AddLocale : LocalizationCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Key of locale to add")]
        public string Key { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Title of locale to add")]
        public string Title { get; set; }

        protected override void ProcessRecord()
        {
            //check for localization enabled
            if (_localizationInfo == null)
            {
                WriteErrorLine("No localization project found. First do Enable-Localization");
                return;
            }
            if (_localizationInfo.LocalesDirectory == null || _localizationInfo.LocalizationDirectory == null || _localizationInfo.LocalizationDesignDataDirectory == null)
            {
                WriteErrorLine("Something wrong with localization settings");
                return;
            }

            if (!IsLocaleKeyValid(Key))
            {
                WriteErrorLine("Locale key shoud has length of minimum 2 and contains only chars");
                return;
            }

            var defaultLocale = GetDefaultLocale();
            if (defaultLocale == null)
            {
                WriteErrorLine("Can't find defaul locale to create new one");
                return;
            }

            AddLocale(Key, Title, defaultLocale.Locale);
        }
    }
}
