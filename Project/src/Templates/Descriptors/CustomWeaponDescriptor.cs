using MGSC;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates.Descriptors;
public class CustomWeaponDescriptor : CustomItemContentDescriptor
{
    public List<string> randomAttachSoundsPaths { get; set; }
    public List<string> randomReloadSoundsPaths { get; set; }
    public List<string> randomDryShotSoundsPaths { get; set; }
    public List<string> randomFailedAttackSoundsPaths { get; set; }
    public List<string> entityFlySpritesPaths { get; set; }
    public string muzzlePrefabPath { get; set; }
    public string overridenBulletPrefabPath { get; set; }

    public override ItemContentDescriptor GetOriginal()
    {
        WeaponDescriptor weaponDescriptor = new WeaponDescriptor()
        {
            // Here all code to get the sounds and images.
        };
        return weaponDescriptor;
    }
}