namespace QM_WeaponImporter.Templates;
public interface ExplosionVisualParameters
{
    public float visualDelay{ get; set; }
    //public ExplosionLightUpdater expLightUpdater{ get; set; }
    // Maybe monobehaviours can execute mode when called? For example call FindObjectOfType when is asked to "report"...
    //public FrameAnimator expAnimator{ get; set; }
    //public SoundBank expSoundBank{ get; set; }
    public bool shakeOnExplosion{ get; set; }
    public int clearGibsRadiusInPixels{ get; set; }
}