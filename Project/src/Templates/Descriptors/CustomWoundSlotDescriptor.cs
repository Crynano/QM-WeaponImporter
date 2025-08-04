using MGSC;
using UnityEngine;

namespace QM_WeaponImporter.Templates;

public class CustomWoundSlotDescriptor : CustomBaseDescriptor
{
    public CustomWoundSlotDescriptor() : base()
    {
        
    }
    
    public Vector2 SlotPosition {get; set;} = Vector2.zero;

    public string NormalIconPath {get; set;} = string.Empty;

    public string WoundedIconPath {get; set;} = string.Empty;

    public string FixatedIconPath {get; set;} = string.Empty;

    public string AmputatedIconPath {get; set;} = string.Empty;
    
    public WoundSlotDescriptor GetOriginal()
    {
        WoundSlotDescriptor original = ScriptableObject.CreateInstance<WoundSlotDescriptor>();

        original._slotPosition = this.SlotPosition;
        original._normalIcon = ItemCreatorHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(NormalIconPath, "NormalIcon", MGSC.Data.WoundSlots) as Sprite ?? Importer.LoadNewSprite(NormalIconPath);
        original._woundedIcon = ItemCreatorHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(WoundedIconPath, "WoundedIcon", MGSC.Data.WoundSlots) as Sprite ?? Importer.LoadNewSprite(WoundedIconPath);
        original._fixatedIcon = ItemCreatorHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(FixatedIconPath, "FixatedIcon", MGSC.Data.WoundSlots) as Sprite ?? Importer.LoadNewSprite(FixatedIconPath);
        original._amputatedIcon = ItemCreatorHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(AmputatedIconPath, "AmputatedIcon", MGSC.Data.WoundSlots) as Sprite ?? Importer.LoadNewSprite(AmputatedIconPath);
        
        return original;
    }
}