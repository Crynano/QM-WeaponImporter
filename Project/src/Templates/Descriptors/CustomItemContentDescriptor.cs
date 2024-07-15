using MGSC;
using System.Collections.Generic;
using UnityEngine;

namespace QM_WeaponImporter.Templates.Descriptors;
public class CustomItemContentDescriptor
{
    public CustomItemContentDescriptor() { }
    public string attachedId { get; set; }
    public string overridenRenderId { get; set; }
    public string iconSpritePath { get; set; }
    public string smallIconSpritePath { get; set; }
    public string shadowOnFloorSpritePath { get; set; }

    public Dictionary<string, string[]> customParameters = new Dictionary<string, string[]>()
    {
        { "sprites", ["Assets/Sprites/weaponSprite01", "Assets/Sprites/weaponSprite02"] },
        { "muzzles", ["Assets/Muzzles/weaponMuzzleAnim01", "Assets/Muzzles/weaponMuzzleAnim01" ] },
        { "soundBank", ["Assets/Sounds/soundBank01", "Assets/Sounds/soundBank01"] },
        { "parametersName", ["RelativePaths1", "RelativePaths2"] }
    };

    public virtual ItemContentDescriptor GetOriginal()
    {
        Logger.WriteToLog($"Returning Original from Parent!");
        ItemContentDescriptor defaultReturn = ScriptableObject.CreateInstance<ItemContentDescriptor>();
        defaultReturn.name = attachedId;
        defaultReturn._overridenRenderId = overridenRenderId;
        defaultReturn._icon = Importer.LoadNewSprite(iconSpritePath);
        defaultReturn._smallIcon = Importer.LoadNewSprite(iconSpritePath);
        defaultReturn._shadow = Importer.LoadNewSprite(iconSpritePath);
        return defaultReturn;
    }
}