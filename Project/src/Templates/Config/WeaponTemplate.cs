using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using MGSC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonProperty(Order = 16)] public int minRandomAmmoCount { get; set; } = 0;
        [JsonProperty(Order = 17)] public string defaultAmmoId { get; set; } = string.Empty;
        [JsonProperty(Order = 18)] public string requiredAmmo { get; set; } = string.Empty;
        [JsonProperty(Order = 19)] public List<string> overrideAmmo { get; set; } = new List<string>();
        [JsonProperty(Order = 20)] public List<string> firemodes { get; set; } = new List<string>();
        [JsonProperty(Order = 27)][JsonConverter(typeof(StringEnumConverter))] public HandsGrip grip { get; set; } = HandsGrip.None;
        [JsonProperty(Order = 28)][JsonConverter(typeof(StringEnumConverter))] public WeaponClass weaponClass { get; set; } = WeaponClass.None;
        [JsonProperty(Order = 29)][JsonConverter(typeof(StringEnumConverter))] public WeaponSubClass weaponSubClass { get; set; } = WeaponSubClass.None;
        [JsonProperty(Order = 30)] public string defaultGrenadeId { get; set; } = string.Empty;
        [JsonProperty(Order = 31)] public List<string> AllowedGrenadeIds { get; set; } = new List<string>();
        [JsonProperty(Order = 32)] public float bonusAccuracy { get; set; } = 0.0f;
        [JsonProperty(Order = 33)] public float bonusScatterAngle { get; set; } = 0.0f;
        [JsonProperty(Order = 45)] public bool hasHFGOverlay { get; set; } = false;
        [JsonProperty(Order = 46)] public bool IsImplicit { get; set; } = false;
        [JsonProperty(Order = 47)] public string OverrideProjectileId { get; set; } = string.Empty;
        [JsonProperty(Order = 51)] public float Falloff { get; set; } = 1.0f;
        [JsonProperty(Order = 52)] public List<string> Traits { get; set; } = new List<string>();

        public WeaponTemplate()
        {
            
        }
    }
}