using MGSC;

namespace QM_WeaponImporter.Templates.Descriptors;
public abstract class CustomItemContentDescriptor
{
    public string attachedId { get; set; }
    public string overridenRenderId { get; set; }
    public string iconSpritePath { get; set; }
    public string smallIconSpritePath { get; set; }
    public string shadowOnFloorSpritePath { get; set; }

    public abstract ItemContentDescriptor GetOriginal();
}
