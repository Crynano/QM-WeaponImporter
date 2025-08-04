using System.Collections.Generic;
using MGSC;
using UnityEngine.PlayerLoop;

namespace QM_WeaponImporter.Templates;
public class AmmoRecordTemplate : ItemRecordTemplate
{
    public AmmoBallisticType BallisticType { get; set; } = AmmoBallisticType.Ballistic;

    public int MinAmmoAmount { get; set; } = 0;

    public int MaxAmmoAmount { get; set; } = 1;

    public short MaxStack { get; set; } = 10;

    public string AmmoType { get; set; } = "BatteryCells";

    public string DmgType { get; set; } = "Cut";

    public float DmgCritChance { get; set; } = 1.0f;

    public int RangeBonus { get; set; } = 1;

    public float AccuracyMult { get; set; } = 0.1f;

    public float ScatterMult { get; set; } = 0.1f;

    public float DamageMult { get; set; } = 1f;

    public int BulletCastsPerShot { get; set; } = 1;

    public string StatusEffectId { get; set; } = "";

    public float ChanceToApply { get; set; } = 0.0f;

    public float StatusDamageModifier { get; set; } = 0.0f;

    public float StatusResistModifier { get; set; } = 0.0f;

    public List<string> Traits { get; set; } = new List<string>();

    public string ProjectileId { get; set; } = "";

    public AmmoRecordTemplate()
    {
        
    }

    public AmmoRecordTemplate(AmmoBallisticType ballisticType, int minAmmoAmount, int maxAmmoAmount, short maxStack, string ammoType, string dmgType, float dmgCritChance, int rangeBonus, float accuracyMult, float scatterMult, float damageMult, int bulletCastsPerShot, string statusEffectId, float chanceToApply, float statusDamageModifier, float statusResistModifier, string projectileId)
    {
        BallisticType = ballisticType;
        MinAmmoAmount = minAmmoAmount;
        MaxAmmoAmount = maxAmmoAmount;
        MaxStack = maxStack;
        AmmoType = ammoType;
        DmgType = dmgType;
        DmgCritChance = dmgCritChance;
        RangeBonus = rangeBonus;
        AccuracyMult = accuracyMult;
        ScatterMult = scatterMult;
        DamageMult = damageMult;
        BulletCastsPerShot = bulletCastsPerShot;
        StatusEffectId = statusEffectId;
        ChanceToApply = chanceToApply;
        StatusDamageModifier = statusDamageModifier;
        StatusResistModifier = statusResistModifier;
        ProjectileId = projectileId;
    }

    public AmmoRecord GetOriginal()
    {
        return new AmmoRecord
        {
            Id = this.Id,
            Categories = this.Categories,
            TechLevel = this.TechLevel,
            Price = this.Price,
            Weight = this.Weight,
            InventoryWidthSize = this.InventoryWidthSize,
            ItemClass = this.ItemClass,
            BallisticType = this.BallisticType,
            MinAmmoAmount = this.MinAmmoAmount,
            MaxAmmoAmount = this.MaxAmmoAmount,
            MaxStack = this.MaxStack,
            AmmoType = this.AmmoType,
            DmgType = this.DmgType,
            DmgCritChance = this.DmgCritChance,
            RangeBonus = this.RangeBonus,
            AccuracyMult = this.AccuracyMult,
            ScatterMult = this.ScatterMult,
            DamageMult = this.DamageMult,
            BulletCastsPerShot = this.BulletCastsPerShot,
            StatusEffectId = this.StatusEffectId,
            ChanceToApply = this.ChanceToApply,
            StatusDamageModifier = this.StatusDamageModifier,
            StatusResistModifier = this.StatusResistModifier,
            Traits = this.Traits,
            ProjectileId = this.ProjectileId
        };
    }
}