using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class MeleeWeaponTemplate : WeaponTemplate
    {
        public bool IsMelee { get; set; }
        public int ThrowRange { get; set; }
        public int durabilityLossOnThrow { get; set; }
        public bool canMeleeAmputate { get; set; }
        public bool GetMeleeDamageFromCreature { get; set; }
        public MeleeWeaponTemplate()
        {
            IsMelee = true;
            range = 1;
        }

        public static MeleeWeaponTemplate GetExample()
        {
            MeleeWeaponTemplate meleeWeapon = new MeleeWeaponTemplate()
            {
                Id = "melee_weapon",
                Price = 100,
                Weight = 0.7f,
                InventoryWidthSize = 2,
                weaponClass = MGSC.WeaponClass.Blade,
                weaponSubClass = MGSC.WeaponSubClass.Default,
                grip = MGSC.HandsGrip.MeleeOneHanded,
                requiredAmmo = "",
                overrideAmmo = new List<string>(),
                defaultAmmoId = "implicted_combat_knife",
                bonusAccuracy = 0.1f,
                minimumDamage = 12,
                maximumDamage = 16,
                criticalChance = 1.5f,
                criticalDamage = 2f,
                firemodes = new List<string>() { "knife_single", "knife_triple" },
                magazineCapacity = 0,
                reloadDuration = 1,
                MaxDurability = 90,
                durabilityLossOnThrow = 1,
                MinDurabilityAfterRepair = 0,
                Unbreakable = false,
                ThrowRange = 5,
                canMeleeAmputate = true,
                range = 1
            };
            return meleeWeapon;
        }
    }
}