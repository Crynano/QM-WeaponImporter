using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class MeleeWeaponTemplate : WeaponTemplate
    {
        /// <summary>
        /// Melee basic properties
        /// </summary>
        public bool isMelee { get; set; }
        public bool doesMeleeSplash { get; set; }
        /// <summary>
        /// Throw properties
        /// </summary>
        public bool canThrow { get; set; }
        public int throwRange { get; set; }
        public bool throwGuaranteedHit { get; set; }
        public bool doesThrowPierce { get; set; }
        public int durabilityLossOnThrow { get; set; }
        /// <summary>
        /// Other melee properties
        /// </summary>
        public bool canMeleeAmputate { get; set; }
        
        public MeleeWeaponTemplate()
        {
            isMelee = true;
            range = 1;
        }

        public static MeleeWeaponTemplate GetExample()
        {
            MeleeWeaponTemplate meleeWeapon = new MeleeWeaponTemplate()
            {
                id = "melee_weapon",
                price = 100,
                weight = 0.7f,
                inventoryWidthSize = 2,
                weaponClass = MGSC.WeaponClass.Blade,
                weaponSubClass = MGSC.WeaponSubClass.Default,
                grip = MGSC.HandsGrip.MeleeOneHanded,
                requiredAmmo = "",
                overrideAmmo = "",
                defaultAmmoId = "implicted_combat_knife",
                bonusAccuracy = 0.1f,
                minimumDamage = 12,
                maximumDamage = 16,
                criticalChance = 1.5f,
                criticalDamage = 2f,
                firemodes = new List<string>() { "knife_single", "knife_triple" },
                magazineCapacity = 0,
                reloadOneBulletAtATime = false,
                reloadDuration = 1,
                maxDurability = 90,
                durabilityLossOnThrow = 1,
                minDurabilityAfterRepair = 0,
                unbreakable = false,
                repairCategory = "pierce_melee",
                doesMeleeSplash = false,
                canThrow = true,
                throwRange = 5,
                throwGuaranteedHit = false,
                doesThrowPierce = false,
                canMeleeAmputate = true,
                amputationOnWound = false,
                range = 1,
                critPainDamageMultiplier = 1f,
                painDamageMultiplier = 1f,
                fovLookAngleMult = 1f
            };
            return meleeWeapon;
        }
    }
}