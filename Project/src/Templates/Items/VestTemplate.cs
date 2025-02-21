using MGSC;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
public class VestTemplate : VestRecord
{
    public Dictionary<string, float> resistSheet = new Dictionary<string, float>();
    public VestTemplate() 
    {
        
    }

    void UpdateResists()
    {
        ResistSheet = new List<DmgResist>();
        foreach (var item in resistSheet)
        {
            ResistSheet.Add(new DmgResist() { damage = item.Key, resistPercent = item.Value });
        }
    }

    public VestRecord GetOriginal()
    {
        UpdateResists();
        VestRecord original = new VestRecord()
        {
            SlotCapacity  =this.SlotCapacity,
            ReloadTurnBonus = this.ReloadTurnBonus,
            DropChanceOnBroken = this.DropChanceOnBroken,
            ResistSheet = this.ResistSheet,
            MaxDurability = this.MaxDurability,
            MinDurabilityAfterRepair = this.MinDurabilityAfterRepair,
            Unbreakable = this.Unbreakable,
            RepairCategory = this.RepairCategory,
            Categories = this.Categories,
            TechLevel = this.TechLevel,
            Price = this.Price,
            Weight = this.Weight,
            InventoryWidthSize = this.InventoryWidthSize,
            ItemClass = this.ItemClass,
            Id = this.Id,
        };
        return original;
    }

    public static VestTemplate GetExample()
    {
        VestTemplate example = new VestTemplate()
        {
            resistSheet = new Dictionary<string, float>()
            {
                { "blunt", 0 },
                { "pierce", 0 },
                { "lacer", 0 },
                { "fire", 0 },
                { "cold", 0 },
                { "poison", 0 },
                { "shock", 0 },
                { "beam", 0 }
            }
        };
        return example;
    }
}
