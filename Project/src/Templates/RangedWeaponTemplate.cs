using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class RangedWeaponTemplate : WeaponTemplate
    {
        /// <summary>
        /// Ranged Properties
        /// </summary>
        public bool rangeExtraThrowback { get; set; }
        public float rangeThrowbackChanceBonus { get; set; }
        public float bonusScatterAngle { get; set; }
        public int minRandomAmmoCount { get; set; }
        public float silencerShotChance { get; set; }
        public int obstaclePierceChanceBonus { get; set; }
        public float creaturePierceBonus { get; set; }
        public float woundChanceOnPierce { get; set; }
        public List<string> AllowedGrenadeIds { get; set; }
        public string defaultGrenadeId { get; set; }

        public RangedWeaponTemplate() 
        {
            AllowedGrenadeIds = new List<string>();
        }

        public static RangedWeaponTemplate GetExample()
        {
            RangedWeaponTemplate rangedWeapon = new RangedWeaponTemplate()
            {
                id = "army_pistol",
                name = "Army Pistol",
                description = "Army Pistol Description",
                price = 100,
                weight = 2f,
                inventoryWidth = 2,
                weaponClass = "Pistol",
                weaponSubClass = "Firearm",
                grip = "Pistol",
                requiredAmmo = "Shells",
                defaultAmmoId = "shotgun_shrapnel_ammo",
                overrideAmmo = "",
                bonusAccuracy = 0.25f,
                minimumDamage = 12,
                maximumDamage = 32,
                criticalChance = 1.5f,
                criticalDamage = 2f,
                firemodes = new List<string>() { "basic_pistol_single", "basic_pistol_burst" },
                range = 3,
                magazineCapacity = 4,
                reloadDuration = 1,
                reloadOneBulletAtATime = true,
                maxDurability = 90,
                minDurabilityAfterRepair = 0,
                unbreakable = false,
                repairCategory = "weapon",
                rangeExtraThrowback = false,
                rangeThrowbackChanceBonus = 0,
                bonusScatterAngle = 0,
                minRandomAmmoCount = 5,
                silencerShotChance = 0,
                obstaclePierceChanceBonus = 0,
                creaturePierceBonus = 0,
                woundChanceOnPierce = 0,
                isSelfCharge = false,
                dotWoundsDamageBonus = 0,
                fractureWoundDamageBonus = 0,
                painDamageMultiplier = 1f,
                critPainDamageMultiplier = 1f,
                offSlotCritChance = 0,
                minDmgCapBonus = 0,
                rampUpValue = 0,
                fovLookAngleMult = 1f,
                hasHFGOverlay = false,
                randomAttackSoundBank = "",
                randomDryShotSoundBank = "",
                randomFailedAttackSoundBank = "",
                randomReloadSoundBank = "",
                iconPath = "Images/Knife.png",
                smallIconPath = "Images/SmallDagger.png",
                shadowOnFloorSpritePath = "folder/filename.extension"
            };
            return rangedWeapon;
        }
    }
}