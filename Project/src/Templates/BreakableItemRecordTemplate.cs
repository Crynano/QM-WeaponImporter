using Newtonsoft.Json;
using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class BreakableItemRecordTemplate : ItemRecordTemplate
{
    [JsonProperty(Order = 23)] public int maxDurability { get; set; } = 0;
    [JsonProperty(Order = 24)]public int minDurabilityAfterRepair { get; set; } = 0;
    [JsonProperty(Order = 25)]public bool unbreakable { get; set; } = false;
    [JsonProperty(Order = 26)]public string repairCategory { get; set; } = string.Empty;
}