using MGSC;

namespace QM_WeaponImporter.Templates.Descriptors;
public class CustomItemContentDescriptor
{
    public CustomItemContentDescriptor() { }
    public string attachedId { get; set; }
    public string overridenRenderId { get; set; }
    public string iconSpritePath { get; set; }
    public string smallIconSpritePath { get; set; }
    public string shadowOnFloorSpritePath { get; set; }

    public ItemContentDescriptor GetOriginal() { return new ItemContentDescriptor(); }
}
