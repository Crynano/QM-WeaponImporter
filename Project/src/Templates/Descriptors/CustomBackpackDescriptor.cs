using MGSC;

namespace QM_WeaponImporter.Templates.Descriptors;
public class CustomBackpackDescriptor : CustomItemContentDescriptor
{
    public CustomBackpackDescriptor() { }
    public override ItemContentDescriptor GetOriginal()
    {
        // Load the assets in the original
        BackpackDescriptor descriptor = new BackpackDescriptor()
        {
            _overridenRenderId = overridenRenderId,
            _icon = Importer.LoadNewSprite(iconSpritePath),
            _smallIcon = Importer.LoadNewSprite(smallIconSpritePath),
            _shadow = Importer.LoadNewSprite(shadowOnFloorSpritePath),
        };
        return descriptor;
    }
}
