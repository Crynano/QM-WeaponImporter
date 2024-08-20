using UnityEngine;

namespace QM_WeaponImporter.Templates;
public abstract class BasePickupItemRecordTemplate : ConfigTableRecordTemplate
{
    public int inventorySortOrder { get; set; } = 100;
}
