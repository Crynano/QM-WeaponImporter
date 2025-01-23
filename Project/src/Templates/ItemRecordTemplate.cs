using System.Collections.Generic;
using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemRecordTemplate : BasePickupItemRecordTemplate
{
    public List<string> categories { get; set; } = new List<string>();
    public int techLevel { get; set; } = 0;
    public float price { get; set; } = 0f;
    public float weight { get; set; } = 0f;
    public int inventoryWidthSize { get; set; } = 1;
    public string itemClass { get; set; } = string.Empty;
}
