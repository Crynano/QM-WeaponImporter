using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using MGSC;
using Newtonsoft.Json;

namespace QM_WeaponImporter
{
    [Serializable]
    // This class is used to expose / map in an easier way the parameters of the weapons.
    // It also includes references to audio and language config so user doesn't need to know how to
    // handle every part of the weapon creation.
    public class WeaponTemplate : BreakableItemRecordTemplate
    {
        [JsonProperty(Order = 9)] public int minimumDamage { get; set; } = 0;
        [JsonProperty(Order = 10)] public int maximumDamage { get; set; } = 0;
        [JsonProperty(Order = 11)] public float criticalChance { get; set; } = 0.0f;
        [JsonProperty(Order = 12)] public float criticalDamage { get; set; } = 0.0f;
        [JsonProperty(Order = 13)] public int range { get; set; } = 0;
        [JsonProperty(Order = 14)] public int magazineCapacity { get; set; } = 0;
        [JsonProperty(Order = 15)] public int reloadDuration { get; set; } = 0;
        [JsonProperty(Order = 16)] public bool reloadOneBulletAtATime { get; set; } = false;
        [JsonProperty(Order = 17)] public string defaultAmmoId { get; set; } = string.Empty;
        [JsonProperty(Order = 18)] public string requiredAmmo { get; set; } = string.Empty;
        [JsonProperty(Order = 19)] public string overrideAmmo { get; set; } = string.Empty;
        [JsonProperty(Order = 20)] public bool isSelfCharge { get; set; } = false;
        [JsonProperty(Order = 21)] public int minRandomAmmoCount { get; set; } = 0;
        [JsonProperty(Order = 22)] public List<string> firemodes { get; set; } = new List<string>();
        //[JsonProperty(Order = 23)] public int maxDurability { get; set; } = 0;
        //[JsonProperty(Order = 24)] public int minDurabilityAfterRepair { get; set; } = 0;
        //[JsonProperty(Order = 25)] public bool unbreakable { get; set; } = false;
        //[JsonProperty(Order = 26)] public string repairCategory { get; set; } = string.Empty;
        [JsonProperty(Order = 27)] public HandsGrip grip { get; set; } = HandsGrip.None;
        [JsonProperty(Order = 28)] public WeaponClass weaponClass { get; set; } = WeaponClass.None;
        [JsonProperty(Order = 29)] public WeaponSubClass weaponSubClass { get; set; } = WeaponSubClass.None;
        [JsonProperty(Order = 30)] public string defaultGrenadeId { get; set; } = string.Empty;
        [JsonProperty(Order = 31)] public List<string> AllowedGrenadeIds { get; set; } = new List<string>();
        [JsonProperty(Order = 32)] public float bonusAccuracy { get; set; } = 0.0f;
        [JsonProperty(Order = 33)] public float bonusScatterAngle { get; set; } = 0.0f;
        [JsonProperty(Order = 34)] public float silencerShotChance { get; set; } = 0.0f;
        [JsonProperty(Order = 35)] public float armorPenetration { get; set; } = 0.0f;
        [JsonProperty(Order = 36)] public float rangeThrowbackChanceBonus { get; set; } = 0.0f;
        [JsonProperty(Order = 37)] public bool rangeExtraThrowback { get; set; } = false;
        [JsonProperty(Order = 38)] public float critPainDamageMultiplier { get; set; } = 0f;
        [JsonProperty(Order = 39)] public float offSlotCritChance { get; set; } = 0f;
        [JsonProperty(Order = 40)] public int rampUpValue { get; set; } = 0;
        [JsonProperty(Order = 41)] public int obstaclePierceChanceBonus { get; set; } = 0;
        [JsonProperty(Order = 42)] public float creaturePierceBonus { get; set; } = 0.0f;
        [JsonProperty(Order = 43)] public float woundChanceOnPierce { get; set; } = 0.0f;
        [JsonProperty(Order = 44)] public float fovLookAngleMult { get; set; } = 0f;
        [JsonProperty(Order = 45)] public int dotWoundsDamageBonus { get; set; } = 0;
        [JsonProperty(Order = 46)] public int fractureWoundDamageBonus { get; set; } = 0;
        [JsonProperty(Order = 47)] public float painDamageMultiplier { get; set; } = 0f;
        [JsonProperty(Order = 48)] public bool amputationOnWound { get; set; } = false;
        [JsonProperty(Order = 49)] public bool hasHFGOverlay { get; set; } = false;
        [JsonProperty(Order = 50)] public bool isImplicit { get; set; } = false;

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