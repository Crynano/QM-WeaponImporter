using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class LocalizationTemplate
    {
        public Dictionary<string, Dictionary<string, string>> name { get; set; }
        public Dictionary<string, Dictionary<string, string>> shortdesc { get; set; }
    }
}