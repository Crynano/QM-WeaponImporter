using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using MGSC;

namespace QM_WeaponImporter
{
    [Serializable]
    // This class is used to expose / map in an easier way the parameters of the weapons.
    // It also includes references to audio and language config so user doesn't need to know how to
    // handle every part of the weapon creation.
    public class WeaponTemplate : BreakableItemRecordTemplate, IWeaponDescriptorTemplate
    {
        /// <summary>
        /// Generic Properties
        /// </summary>
        public bool isImplicit { get; set; } = false;
        public HandsGrip grip { get; set; } = HandsGrip.None;
        public WeaponClass weaponClass { get; set; } = WeaponClass.None;
        public WeaponSubClass weaponSubClass { get; set; } = WeaponSubClass.None;
        public string requiredAmmo { get; set; } = string.Empty;
        public string overrideAmmo { get; set; } = string.Empty;
        public string defaultAmmoId { get; set; } = string.Empty;
        public float bonusAccuracy { get; set; } = 0.0f;
        public int range { get; set; } = 0;
        public int minimumDamage { get; set; } = 0;
        public int maximumDamage { get; set; } = 0;
        public float armorPenetration { get; set; } = 0.0f;
        public float criticalChance { get; set; } = 0.0f;
        public float criticalDamage { get; set; } = 0.0f;
        public List<string> firemodes { get; set; } = new List<string>();
        public int magazineCapacity { get; set; } = 0;
        public int reloadDuration { get; set; } = 0;
        public bool reloadOneBulletAtATime { get; set; } = false;
        public bool isSelfCharge { get; set; } = false;
        public int dotWoundsDamageBonus { get; set; } = 0;
        public int fractureWoundDamageBonus { get; set; } = 0;
        public float painDamageMultiplier { get; set; } = 0f;
        public float critPainDamageMultiplier { get; set; } = 0f;
        public float offSlotCritChance { get; set; } = 0f;
        public float minDmgCapBonus { get; set; } = 0f;
        public int rampUpValue { get; set; } = 0;
        public float fovLookAngleMult { get; set; } = 0f;

        /// <summary>
        /// WeaponDescriptorParameters
        /// </summary>
       
        // This properties get moved to another descriptors
        public float visualReachCellDuration { get; set; } = 0f;
        public List<string> entityFlySprites { get; set; } = new List<string>();
        public bool useCustomBullet { get; set; } = false;
        public string bulletAssetPath { get; set; } = string.Empty;
        public bool hasHFGOverlay { get; set; } = false;

        public WeaponTemplate()
        {
            // Setting those variables because they should be 1 by default.
            // Not initializing them results in the game assuming they are 0, which is detrmiental and causes
            // unexpected and unwanted behaviour on the weapons.
            critPainDamageMultiplier = 1f;
            painDamageMultiplier = 1f;
            fovLookAngleMult = 1f;
            armorPenetration = 0f;
        }
    }
}