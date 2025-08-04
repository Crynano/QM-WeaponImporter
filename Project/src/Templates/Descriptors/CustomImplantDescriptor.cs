using MGSC;

namespace QM_WeaponImporter.Templates;

public class CustomImplantDescriptor : CustomItemContentDescriptor
{
    public CustomImplantDescriptor() : base()
    {
        
    }
    
    public string UseSoundPath {get; set;} = string.Empty;
    
    public ImplantDescriptor GetOriginal()
    {
        ImplantDescriptor original = base.GetOriginal<ImplantDescriptor>();

        original._useSound = ItemCreatorHelper.GetPropertyFromItem<ImplantDescriptor>(UseSoundPath, "UseSound") as SoundBank ?? null;
        
        return original;
    }
}