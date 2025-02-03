using MGSC;
using System.Collections.Generic;
using UnityEngine;

namespace QM_WeaponImporter.Templates;
public class CustomItemContentDescriptor
{
    public CustomItemContentDescriptor() { }
    public string attachedId { get; set; } = string.Empty;
    public string baseItemId { get; set; } = string.Empty;
    public string overridenRenderId { get; set; } = string.Empty;
    public string iconSpritePath { get; set; } = string.Empty;
    public string smallIconSpritePath { get; set; } = string.Empty;
    public string shadowOnFloorSpritePath { get; set; } = string.Empty;
    public string shootSoundPath { get; set; } = string.Empty;
    public string dryShotSoundPath { get; set; } = string.Empty;
    public string failedAttackSoundPath { get; set; } = string.Empty;
    public string reloadSoundPath { get; set; } = string.Empty;
    public string bundlePath { get; set; } = string.Empty;
    public string prefabName { get; set; } = string.Empty;
    public string textureName {  get; set; } = string.Empty;

    public Dictionary<string, string[]> customParameters = new Dictionary<string, string[]>()
    {
        { "sprites", ["Assets/Sprites/weaponSprite01", "Assets/Sprites/weaponSprite02"] }
    };

    public virtual T GetOriginal<T>() where T : ItemContentDescriptor
    {
        T defaultReturn = ScriptableObject.CreateInstance<T>();
        defaultReturn._overridenRenderId = overridenRenderId;
        defaultReturn._icon = Importer.LoadNewSprite(iconSpritePath);
        defaultReturn._smallIcon = Importer.LoadNewSprite(iconSpritePath);
        defaultReturn._shadow = Importer.LoadNewSprite(iconSpritePath);
        return defaultReturn;
    }
}