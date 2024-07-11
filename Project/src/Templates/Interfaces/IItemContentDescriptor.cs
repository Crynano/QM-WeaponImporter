using UnityEngine;

namespace QM_WeaponImporter.Templates;

// Use this interface so a Class has an image reference without building up clutter.
internal interface IItemContentDescriptor
{
    public string overridenRenderId { get; set; }
    public string iconPath { get; set; }
    public string smallIconPath { get; set; }
    public string shadowOnFloorSpritePath { get; set; }
}
