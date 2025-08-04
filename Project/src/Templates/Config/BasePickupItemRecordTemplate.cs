using Newtonsoft.Json;
using UnityEngine;

namespace QM_WeaponImporter.Templates;
public abstract class BasePickupItemRecordTemplate : ConfigTableRecordTemplate
{
    [JsonProperty(Order = 2)]
    public int InventorySortOrder { get; set; } = 100;
}