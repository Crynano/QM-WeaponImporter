using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
public interface IWeaponDescriptorTemplate
{
    public string grip { get; set; }
    public List<string> randomAttackSoundBank { get; set; }
    public List<string> randomDryShotSoundBank { get; set; }
    public List<string> randomFailedAttackSoundBank { get; set; }
    public List<string> randomReloadSoundBank { get; set; }
    public float visualReachCellDuration { get; set; }
    public List<string> entityFlySprites { get; set; }
    public bool useCustomBullet { get; set; }

    /// <summary>
    /// All bullet types:
    /// ThermalBullet
    /// PlasmaBullet
    /// HFGBullet
    /// ToxicThrowerBullet
    /// ContinuousLaserBullet
    /// FrostBullet
    /// QuasiBeamBullet
    /// PlasmaCharge
    /// ContinuousQuasiLaserBullet
    /// TechFrostBullet
    /// LightingBullet
    /// QuasiBeam
    /// PistolBullet
    /// ShrapnelBullet
    /// LaserBullet
    /// DiskBullet
    /// QuasiBolt
    /// ToxicBullet
    /// FlameThrowerBullet
    /// </summary>

    public string bulletAssetPath { get; set; }
    public bool hasHFGOverlay { get; set; }
}