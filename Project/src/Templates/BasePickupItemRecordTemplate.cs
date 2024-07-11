using UnityEngine;

namespace QM_WeaponImporter.Templates;
public abstract class BasePickupItemRecordTemplate : ConfigTableRecordTemplate, IItemContentDescriptor
{
    public int inventorySortOrder { get; set; } = 100;
    public string overridenRenderId { get; set; }
    public string iconPath { get; set; }
    public string smallIconPath { get; set; }
    public string shadowOnFloorSpritePath { get; set; }
}
