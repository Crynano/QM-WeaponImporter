using MGSC;

namespace QM_WeaponImporter.Templates;
public class GrenadeTemplate : GrenadeRecord
{
    public GrenadeTemplate() { }
    public static GrenadeTemplate GetExample()
    {
        GrenadeTemplate grenade = new GrenadeTemplate()
        {
            Id = "test_grenade",
        };
        return grenade;
    }
}
