using MGSC;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
public class ArmorTemplate : ArmorRecord
{
    public Dictionary<string, float> resistSheet = new Dictionary<string, float>();
    public ArmorTemplate()
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

    public ArmorRecord GetOriginal()
    {
        UpdateResists();
        ArmorRecord original = new ArmorRecord()
        {
            InventoryWidthSize = this.InventoryWidthSize,
            ResistSheet = this.ResistSheet,
            MaxDurability = this.MaxDurability,
            MinDurabilityAfterRepair = this.MinDurabilityAfterRepair,
            Unbreakable = this.Unbreakable,
            RepairCategory = this.RepairCategory,
            Price = this.Price,
            Weight = this.Weight,
            Id = this.Id,
        };
        return original;
    }

    public static ArmorTemplate GetExample()
    {
        ArmorTemplate example = new ArmorTemplate()
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
