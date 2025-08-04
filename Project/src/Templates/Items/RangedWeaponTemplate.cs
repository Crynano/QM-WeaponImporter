using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class RangedWeaponTemplate : WeaponTemplate
    {
        public RangedWeaponTemplate() 
        {
            AllowedGrenadeIds = new List<string>();
        }

        public static RangedWeaponTemplate GetExample()
        {
            RangedWeaponTemplate rangedWeapon = new RangedWeaponTemplate()
            {
                Id = "army_pistol",
                Price = 100,
                Weight = 2f,
                InventoryWidthSize = 2,
                weaponClass = MGSC.WeaponClass.Pistol,
                weaponSubClass = MGSC.WeaponSubClass.Firearm,
                grip = MGSC.HandsGrip.Pistol,
                requiredAmmo = "Shells",
                defaultAmmoId = "shotgun_shrapnel_ammo",
                overrideAmmo = new List<string>() { "" , "" },
                bonusAccuracy = 0.25f,
                minimumDamage = 12,
                maximumDamage = 32,
                criticalChance = 1.5f,
                criticalDamage = 2f,
                firemodes = new List<string>() { "basic_pistol_single", "basic_pistol_burst" },
                range = 3,
                magazineCapacity = 4,
                reloadDuration = 1,
                MaxDurability = 90,
                MinDurabilityAfterRepair = 0,
                Unbreakable = false,
                RepairItemIds = new List<string>(),
                bonusScatterAngle = 0,
                minRandomAmmoCount = 5,
                hasHFGOverlay = false
            };
            return rangedWeapon;
        }
    }
}