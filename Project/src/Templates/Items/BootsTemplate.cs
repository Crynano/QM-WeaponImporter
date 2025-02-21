using MGSC;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
public class BootsTemplate : BootsRecord
{
    public Dictionary<string, float> resistSheet = new Dictionary<string, float>();
    public BootsTemplate()
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

    public BootsRecord GetOriginal()
    {
        UpdateResists();
        BootsRecord original = new BootsRecord()
        {
            ArmorClass = this.ArmorClass,
            ArmorSubClass = this.ArmorSubClass,
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

    public static BootsTemplate GetExample()
    {
        BootsTemplate example = new BootsTemplate()
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
