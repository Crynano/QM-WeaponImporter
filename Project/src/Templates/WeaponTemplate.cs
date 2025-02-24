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
    public class WeaponTemplate : ItemRecordTemplate
    {
        public int minimumDamage { get; set; } = 0;
        public int maximumDamage { get; set; } = 0;
        public float criticalChance { get; set; } = 0.0f;
        public float criticalDamage { get; set; } = 0.0f;
        public int range { get; set; } = 0;
        public int magazineCapacity { get; set; } = 0;
        public int reloadDuration { get; set; } = 0;
        public bool reloadOneBulletAtATime { get; set; } = false;
        public string defaultAmmoId { get; set; } = string.Empty;
        public string requiredAmmo { get; set; } = string.Empty;
        public string overrideAmmo { get; set; } = string.Empty;
        public bool isSelfCharge { get; set; } = false;
        public int minRandomAmmoCount { get; set; } = 0;
        public List<string> firemodes { get; set; } = new List<string>();
        public int maxDurability { get; set; } = 0;
        public int minDurabilityAfterRepair { get; set; } = 0;
        public bool unbreakable { get; set; } = false;
        public string repairCategory { get; set; } = string.Empty;
        public HandsGrip grip { get; set; } = HandsGrip.None;
        public WeaponClass weaponClass { get; set; } = WeaponClass.None;
        public WeaponSubClass weaponSubClass { get; set; } = WeaponSubClass.None;
        public string defaultGrenadeId { get; set; } = string.Empty;
        public List<string> AllowedGrenadeIds { get; set; } = new List<string>();
        public float bonusAccuracy { get; set; } = 0.0f;
        public float bonusScatterAngle { get; set; } = 0.0f;
        public float silencerShotChance { get; set; } = 0.0f;
        public float armorPenetration { get; set; } = 0.0f;
        public float rangeThrowbackChanceBonus { get; set; } = 0.0f;
        public bool rangeExtraThrowback { get; set; } = false;
        public float critPainDamageMultiplier { get; set; } = 0f;
        public float offSlotCritChance { get; set; } = 0f;
        public int rampUpValue { get; set; } = 0;
        public int obstaclePierceChanceBonus { get; set; } = 0;
        public float creaturePierceBonus { get; set; } = 0.0f;
        public float woundChanceOnPierce { get; set; } = 0.0f;
        public float fovLookAngleMult { get; set; } = 0f;
        public int dotWoundsDamageBonus { get; set; } = 0;
        public int fractureWoundDamageBonus { get; set; } = 0;
        public float painDamageMultiplier { get; set; } = 0f;
        public bool amputationOnWound { get; set; } = false;
        public bool hasHFGOverlay { get; set; } = false;
        public bool isImplicit { get; set; } = false;

        // This properties get moved to another descriptors
        //public float visualReachCellDuration { get; set; } = 0f;
        //public List<string> entityFlySprites { get; set; } = new List<string>();
        //public bool useCustomBullet { get; set; } = false;
        //public string bulletAssetPath { get; set; } = string.Empty;

        public WeaponTemplate()
        {
            
        }
    }
}