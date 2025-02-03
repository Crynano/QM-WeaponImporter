using MGSC;

namespace QM_WeaponImporter.Templates;
public class AmmoRecordTemplate : ItemRecordTemplate
{
    public int InventorySortOrder { get; set; } = 8;

    public bool IsImplictedAmmo { get; set; } = false;

    public bool ThrowBackTarget => ThrowBackChance > 0f;

    public AmmoBallisticType BallisticType { get; set; } = AmmoBallisticType.Ballistic;

    public int MinAmmoAmount { get; set; } = 0;

    public int MaxAmmoAmount { get; set; } = 0;

    public short MaxStack { get; set; } = 0;

    public string AmmoType { get; set; } = string.Empty;

    public string DmgType { get; set; } = string.Empty;

    public float DmgCritChance { get; set; } = 0f;

    public float AccuracyMult { get; set; } = 0f;

    public float DamageMult { get; set; } = 0f;

    public int BulletCastsPerShot { get; set; } = 1;

    public float ThrowBackChance { get; set; } = 0f;

    public float TargetBurningChance { get; set; } = 0f;

    public float TileBurningChance { get; set; } = 0f;

    public float TileToxicLiquidChance { get; set; } = 0f;

    public float PierceCreaturesChance { get; set; } = 0f;

    public float StunChance { get; set; } = 0f;

    public int StunDuration { get; set; } = 0;
}