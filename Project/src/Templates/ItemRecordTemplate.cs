using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemRecordTemplate : BasePickupItemRecordTemplate
{
    public float price { get; set; }
    public float weight { get; set; }
    public int inventoryWidthSize { get; set; } = 1;
}
