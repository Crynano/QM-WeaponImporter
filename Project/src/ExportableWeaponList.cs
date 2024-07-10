using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class ExportableWeaponList
    {
        // Gonna K.I.S.S. it
        private List<MeleeWeaponTemplate> meleeWeapons = new List<MeleeWeaponTemplate>();
        private List<RangedWeaponTemplate> rangedWeapons = new List<RangedWeaponTemplate>();

        public List<MeleeWeaponTemplate> Melee { get => meleeWeapons; set => meleeWeapons = value; }
        public List<RangedWeaponTemplate> Ranged { get => rangedWeapons; set => rangedWeapons = value; }

        public static ExportableWeaponList GetExample()
        {
            var exampleList = new ExportableWeaponList();
            exampleList.Melee = new List<MeleeWeaponTemplate>
            {
                MeleeWeaponTemplate.GetExample()
            };
            exampleList.Ranged = new List<RangedWeaponTemplate>
            {
                RangedWeaponTemplate.GetExample()
            };
            return exampleList;
        }
    }

    [Serializable]
    public class ExportableFactionList
    {
        private List<FactionTemplate> factions = new List<FactionTemplate>();

        public List<FactionTemplate> Factions { get => factions; set => factions = value; }

        public static ExportableFactionList GetExample()
        {
            var exampleList = new ExportableFactionList();
            exampleList.Factions = new List<FactionTemplate>
            {
                FactionTemplate.GetExample()
            };
            return exampleList;
        }
    }
}
