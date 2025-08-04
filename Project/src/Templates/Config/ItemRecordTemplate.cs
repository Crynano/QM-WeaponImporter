using System.Collections.Generic;
using System;
using System.ComponentModel;
using MGSC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemRecordTemplate : BasePickupItemRecordTemplate
{
    [JsonProperty(Order = 3)]
    [JsonConverter(typeof(StringEnumConverter))]
    public ItemClass ItemClass { get; set; } = ItemClass.None;
    [JsonProperty(Order = 4)]
    public int TechLevel { get; set; } = 0;
    [JsonProperty(Order = 5)]
    public float Price { get; set; } = 0f;
    [JsonProperty(Order = 6)]
    public float Weight { get; set; } = 0f;
    [JsonProperty(Order = 7)]
    public int InventoryWidthSize { get; set; } = 1;
    [JsonProperty(Order = 8)]
    public List<string> Categories { get; set; } = new List<string>();
}
