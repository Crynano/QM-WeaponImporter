using System.Collections.Generic;
using System;
using MGSC;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemRecordTemplate : BasePickupItemRecordTemplate
{
    public List<string> categories { get; set; } = new List<string>();
    public int techLevel { get; set; } = 0;
    public float price { get; set; } = 0f;
    public float weight { get; set; } = 0f;
    public int inventoryWidthSize { get; set; } = 1;
    public ItemClass itemClass { get; set; } = ItemClass.None;
}
