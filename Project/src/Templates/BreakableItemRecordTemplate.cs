using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class BreakableItemRecordTemplate : ItemRecordTemplate
{
    public int maxDurability { get; set; }
    public int minDurabilityAfterRepair { get; set; }
    public bool unbreakable { get; set; }
    public string repairCategory { get; set; }
}