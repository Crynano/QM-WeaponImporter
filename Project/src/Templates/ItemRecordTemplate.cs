using System.Collections.Generic;
using System;
using MGSC;
using Newtonsoft.Json;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemRecordTemplate : BasePickupItemRecordTemplate
{
    [JsonProperty(Order = 3)]
    public ItemClass itemClass { get; set; } = ItemClass.None;
    [JsonProperty(Order = 4)]
    public int techLevel { get; set; } = 0;
    [JsonProperty(Order = 5)]
    public float price { get; set; } = 0f;
    [JsonProperty(Order = 6)]
    public float weight { get; set; } = 0f;
    [JsonProperty(Order = 7)]
    public int inventoryWidthSize { get; set; } = 1;
    [JsonProperty(Order = 8)]
    public List<string> categories { get; set; } = new List<string>();
}
