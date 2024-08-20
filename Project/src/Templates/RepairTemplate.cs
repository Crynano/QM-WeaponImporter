using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
internal class RepairTemplate : MGSC.RepairRecord
{
    public static MGSC.RepairRecord GetExample()
    {
        MGSC.RepairRecord example = new MGSC.RepairRecord()
        {
            RepairCategories = new List<string>()
            {
                "all"
            }
        };
        return example;
    }
}