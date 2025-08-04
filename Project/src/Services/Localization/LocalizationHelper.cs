using System.Collections.Generic;
using MGSC;

namespace QM_WeaponImporter
{
    public static class LocalizationHelper
    {
        public static void AddLocToAllDictionaries(string fullyQualifiedKey, string text)
        {
            Dictionary<MGSC.Localization.Lang, Dictionary<string, string>> localizationDb = MGSC.Localization.Instance.db;

            foreach (var dictionary in localizationDb)
            {
                dictionary.Value.Add(fullyQualifiedKey, text);
            }
        }

        public static void AddLocalization(string fullyQualifiedKey, string text, Localization.Lang language)
        {
            MGSC.Localization.Instance.db[language].Add(fullyQualifiedKey, text);
        }
    }
}