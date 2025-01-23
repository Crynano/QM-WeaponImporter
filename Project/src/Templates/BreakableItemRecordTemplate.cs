using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class BreakableItemRecordTemplate : ItemRecordTemplate
{
    public int maxDurability { get; set; } = 0;
    public int minDurabilityAfterRepair { get; set; } = 0;
    public bool unbreakable { get; set; } = false;
    public string repairCategory { get; set; } = string.Empty;
}