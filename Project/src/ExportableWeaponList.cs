using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class ExportableWeaponList
    {
        // Gonna K.I.S.S. it
        private List<WeaponTemplate> weapons = new List<WeaponTemplate>();

        public List<WeaponTemplate> Weapons { get => weapons; set => weapons = value; }

        public static ExportableWeaponList GetExampleWeapon()
        {
            var exampleList = new ExportableWeaponList();
            exampleList.Weapons = new List<WeaponTemplate>
            {
                new WeaponTemplate(true)
            };
            return exampleList;
        }
    }
}
