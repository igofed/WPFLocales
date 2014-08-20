using System.Linq;
using System.Management.Automation;
using WPFLocales.Powershell.Properties;
using WPFLocales.Powershell.Utils;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsData.Sync, "Localization")]
    public class SyncLocalization : LocalizationCmdlet
    {
        protected override void ProcessRecord()
        {
            //check for localization enabled
            if (_localizationInfo == null)
            {
                WriteErrorLine("No localization project found. First do Enable-Localization");
                return;
            }

            var defaultLocale = GetDefaultLocale();
            if (defaultLocale == null)
            {
                WriteErrorLine("No default locale found");
                return;
            }

            if (defaultLocale.Locale.Groups == null)
            {
                WriteErrorLine("Default locale has no groups");
                return;
            }

            //check default locale for duplicate groups
            var duplicateGroups = defaultLocale.Locale.Groups.GroupBy(g => g.Key).Where(g => g.Count() > 1).ToArray();
            if (duplicateGroups.Length > 0)
            {
                WriteErrorLine(string.Format("Default locale has duplicate groups: {0}", string.Join(",", duplicateGroups.Select(d => d.Key))));
                return;
            }

            //check default locale for duplicate items in groups
            foreach (var group in defaultLocale.Locale.Groups.Where(g => g.Items != null))
            {
                var duplicateItems = group.Items.GroupBy(i => i.Key).ToArray();
                if (duplicateItems.Length > 0)
                {
                    WriteErrorLine(string.Format("Default locale has duplicate items in group {0}: {1}", group.Key, string.Join(",", duplicateGroups.Select(d => d.Key))));
                    return;
                }
            }

            //sync localekeys file
            var localizationKeysFileText = Templates.TemplatesHelper.GenerateLocalizationKeysFileText(_localizationInfo.Project.GetRootNamespace(), _localizationInfo.LocalizationNamespace, defaultLocale.Locale);
            _localizationInfo.LocalizationKeys.ChangeContent(localizationKeysFileText);


            foreach (var localeInfo in GetAlLocales())
            {
                //sync locales files with default locale
                if (!localeInfo.Locale.IsDefault)
                {
                    //find groups, whick missed in non default locale, but exist in default one and add them
                    var missedGroups = localeInfo.Locale.Groups.Where(g => !defaultLocale.Locale.Groups.Any(dg => dg.Key == g.Key)).ToArray();
                    foreach (var missedGroup in missedGroups)
                    {
                        localeInfo.Locale.Groups.Add(missedGroup);

                        WriteLine(string.Format("Group {0} added to locale {1}", missedGroup.Key, localeInfo.Locale.Key));
                    }

                    foreach (var group in localeInfo.Locale.Groups)
                    {
                        //looking for group in default locale
                        var defaultGroup = defaultLocale.Locale.Groups.FirstOrDefault(dg => dg.Key == group.Key);
                        if (defaultGroup == null)
                        {
                            WriteWarning(string.Format("Group {0} exists in locale {1}, but missing in default locale ({2})", group.Key, localeInfo.Locale.Key, defaultLocale.Locale.Key));
                            continue;
                        }

                        //looking for items missing in default locale
                        foreach (var item in group.Items.Where(i => !defaultGroup.Items.Any(di => di.Key == i.Key)))
                        {
                            WriteWarning(string.Format("Item {0} exists in locale {1} in group {2}, but missing in default locale ({3}) in same group", item.Key, localeInfo.Locale.Key, group.Key, defaultLocale.Locale.Key));
                        }

                        //looking for items missing in locale and add them
                        var defaultItemsMissingInLocale = defaultGroup.Items.Where(di => !group.Items.Any(i => i.Key == di.Key));
                        foreach (var item in defaultItemsMissingInLocale)
                        {
                            group.Items.Add(item);
                        }
                    }

                    //todo: write locale to file
                }

                //sync designTimeLocale file
                var designTimeLocaleFileText = Templates.TemplatesHelper.GenerateDesignTimeLocaleFileText(localeInfo.Locale.Key, localeInfo.Locale.Title, _localizationInfo.Project.GetRootNamespace(), _localizationInfo.LocalizationNamespace, Resources.DesignTimeDataDirectoryName, localeInfo.Locale);
                localeInfo.DesignTimeLocaleItem.ChangeContent(designTimeLocaleFileText);
                WriteLine(string.Format(@"Design time locale for ""{0}"" synced", localeInfo.Locale.Key));
            }
        }
    }
}
