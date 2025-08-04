using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class BreakableItemRecordTemplate : ItemRecordTemplate
{
    [JsonProperty(Order = 23)] public int MaxDurability { get; set; } = 0;
    [JsonProperty(Order = 24)]public int MinDurabilityAfterRepair { get; set; } = 0;
    [JsonProperty(Order = 25)]public bool Unbreakable { get; set; } = false;
    [JsonProperty(Order = 26)] public List<string> RepairItemIds { get; set; } = new List<string>();
}