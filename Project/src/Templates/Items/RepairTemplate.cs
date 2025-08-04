using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
internal class RepairTemplate : MGSC.RepairRecord
{
    public static MGSC.RepairRecord GetExample()
    {
        MGSC.RepairRecord example = new MGSC.RepairRecord()
        {
            RepairSpecialRule = MGSC.RepairSpecialRule.All,
            RestoreAmount = 1,
            MaxStack = 1,
            MaxCapacity = 1,
        };
        return example;
    }
}