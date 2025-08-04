using MGSC;
using UnityEngine;

namespace QM_WeaponImporter.Templates
{
    public class CustomItemContentDescriptor : CustomBaseDescriptor
    {
        public CustomItemContentDescriptor() : base() { }

        public string iconSpritePath { get; set; } = string.Empty;
        public string smallIconSpritePath { get; set; } = string.Empty;
        public string shadowOnFloorSpritePath { get; set; } = string.Empty;

        public T GetOriginal<T>() where T : ItemContentDescriptor
        {
            T defaultReturn = ScriptableObject.CreateInstance<T>();
            defaultReturn._overridenRenderId = overridenRenderId;
            defaultReturn._icon = ItemCreatorHelper.GetPropertyFromItem<AmmoDescriptor>(iconSpritePath, "Icon") as Sprite ?? Importer.LoadNewSprite(iconSpritePath);
            defaultReturn._smallIcon = ItemCreatorHelper.GetPropertyFromItem<AmmoDescriptor>(smallIconSpritePath, "SmallIcon") as Sprite ?? Importer.LoadNewSprite(smallIconSpritePath);
            defaultReturn._shadow = ItemCreatorHelper.GetPropertyFromItem<AmmoDescriptor>(shadowOnFloorSpritePath, "ShadowOnFloor") as Sprite ?? Importer.LoadNewSprite(shadowOnFloorSpritePath);
            return defaultReturn;
        }
    }
}