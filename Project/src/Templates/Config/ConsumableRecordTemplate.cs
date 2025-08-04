using MGSC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ConsumableRecordTemplate : UsableItemRecordTemplate
{
    [JsonProperty(Order = 11)]
    public string GarbageItemId { get; set; } = string.Empty;

    [JsonProperty(Order = 12)]
    public short MaxStack { get; set; } = 0;

    [JsonProperty(Order = 13)]
    public int StarvationValue { get; set; } = 0;

    [JsonProperty(Order = 14)]
    public int HealthValue { get; set; } = 0;

    [JsonProperty(Order = 15)]
    public int HealDuration { get; set; } = 0;

    [JsonProperty(Order = 16)]
    public int MaxHealthValue { get; set; } = 0;

    [JsonProperty(Order = 17)]
    public int QmorphosValue { get; set; } = 0;

    [JsonProperty(Order = 18)]
    public int PainValue { get; set; } = 0;

    [JsonProperty(Order = 19)]
    public int PainDuration { get; set; } = 0;

    [JsonProperty(Order = 20)]
    public float FixAllWoundsChance { get; set; } = 0f;

    [JsonProperty(Order = 21)]
    public float HealAllWoundsChance { get; set; } = 0f;

    [JsonProperty(Order = 22)]
    public Dictionary<string, float> FixWeights { get; set; } = new Dictionary<string, float>();

    [JsonProperty(Order = 23)]
    public Dictionary<string, float> Buffs { get; set; } = new Dictionary<string, float>();

    [JsonProperty(Order = 24)]
    public Dictionary<string, float> EffectChance { get; set; } = new Dictionary<string, float>();

    [JsonProperty(Order = 25)]
    public Dictionary<string, int> EffectProgression { get; set; } = new Dictionary<string, int>();

    public static ConsumableRecordTemplate GetExample()
    {
        return new ConsumableRecordTemplate()
        {
            Id = "example_consumable_id"
        };
    }
}
