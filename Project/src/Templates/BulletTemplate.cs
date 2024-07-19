using System.Collections.Generic;

namespace QM_WeaponImporter.Templates;
public class BulletTemplate
{
    public float bulletSpeed { get; set; }
    public bool makeBloodDecals { get; set; }
    public bool putShotDecalsOnWalls { get; set; }
    public bool putBulletShellsOnFloor { get; set; }
    public bool rotateBulletInShotDir { get; set; } = true;
    public float shakeDuration { get; set; } = 0.25f;
    public float shakeStrength { get; set; } = 0.1f;
    public List<string> facadeDecals { get; set; } = new List<string>();

    // Same thing, when asked for this, just FindObjectOfType???? Who knows if it works.
    // public string gibsController? a reference to the main gibsController?
    public bool putDecals { get; set; } = true;
}
