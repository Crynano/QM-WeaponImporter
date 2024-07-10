using System;
using System.Collections.Generic;

/* The following are, or should be, all variables from a WeaponDescriptor
Id
Price
Weight
IsMelee
CanThrow
ThrowAutoHit
ThrowPierce
RangeThrowbackChanceBonus
RangeExtraThrowback
MeleeSplash
MeleeCanAmputate
AmputationOnWound
DotWoundsDamageBonus
FractureWoundDamageBonus
PainDamageMultiplier
CritPainDamageMultiplier
OffSlotCritChance
InventoryWidthSize
WeaponClass
WeaponSubClass
RequiredAmmo
OverrideAmmo
DefaultAmmoId
MinDamage
MaxDamage
CritChance
CritDamage
BonusAccuracy
BonusScatterAngle
FovLookAngleMult
ObstaclePierceChanceBonus
CreaturePierceBonus
WoundChanceOnPierce
Firemodes
Range
MagazineCapacity
ReloadDuration
ReloadOneClip
SilencerShotChance
DurabilityLossOnThrow
ThrowRange
MaxDurability
MinDurabilityAfterRepair
Unbreakable
RepairCategory
DefaultGrenadeId
MinRandomAmmoCount
AllowedGrenadeIds
IsSelfCharge
MinDmgCapBonus
RampUpValue
*/

namespace QM_WeaponImporter
{
    [Serializable]
    // This class is used to expose / map in an easier way the parameters of the weapons.
    // It also includes references to audio and language config so user doesn't need to know how to
    // handle every part of the weapon creation.
    public class WeaponTemplate
    {
        /// <summary>
        /// Generic Properties
        /// </summary>
        public string id{ get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int price{ get; set; }
        public float weight{ get; set; }
        public int inventoryWidth{ get; set; }
        public string weaponClass{ get; set; }
        public string weaponSubClass{ get; set; }
        public string requiredAmmo{ get; set; }
        public string overrideAmmo{ get; set; }
        public string defaultAmmoId{ get; set; }
        public string grip { get; set; }
        public float bonusAccuracy{ get; set; }
        public int range { get; set; }
        public int minimumDamage{ get; set; }
        public int maximumDamage{ get; set; }
        public float criticalChance{ get; set; }
        public float criticalDamage{ get; set; }
        public List<string> firemodes{ get; set; }
        public int magazineCapacity{ get; set; }
        public int reloadDuration{ get; set; }
        public bool reloadOneBulletAtATime{ get; set; }
        public int maxDurability{ get; set; }
        public int minDurabilityAfterRepair{ get; set; }
        public bool unbreakable{ get; set; }
        public string repairCategory{ get; set; }
        public bool hasHFGOverlay { get; set; }
        //public List<MGSC.Muzzle> muzzleList { get; set; } ??? WHAT IS THIS? MUZZLE EXPLOSION?
        //public MGSC.CommonBullet overridenBulletPrefab { get; set; } ALSO RANGED? WHAT?
        public string randomAttackSoundBank { get; set; }
        public string randomDryShotSoundBank { get; set; }
        public string randomFailedAttackSoundBank { get; set; }
        public string randomReloadSoundBank { get; set; }
        /// <summary>
        /// Strange properties
        /// </summary>
        public bool isSelfCharge{ get; set; }
        public int dotWoundsDamageBonus{ get; set; }
        public int fractureWoundDamageBonus{ get; set; }
        public float painDamageMultiplier{ get; set; }
        public float critPainDamageMultiplier{ get; set; }
        public float offSlotCritChance{ get; set; }
        public float minDmgCapBonus{ get; set; }
        public int rampUpValue{ get; set; }
        public float fovLookAngleMult{ get; set; }

        /// <summary>
        /// Visuals
        /// </summary>
        public string iconPath { get; set; }
        public string smallIconPath { get; set; }
        public string shadowOnFloorSpritePath { get; set; }
        /// <summary>
        /// Localization
        /// </summary>

        public WeaponTemplate()
        {
            // Setting those variables because they should be 1 by default.
            // Not initializing them results in the game assuming they are 0, which is detrmiental and causes
            // unexpected and unwanted behaviour on the weapons.
            critPainDamageMultiplier = 1f;
            painDamageMultiplier = 1f;
            fovLookAngleMult = 1f;
        }

        //public static WeaponTemplate GetExample()
        //{
        //    // The defaultest default
        //    WeaponTemplate weaponTemplate = new WeaponTemplate()
        //    {
        //        id = "army_knife",
        //        name = "Default name",
        //        description = "Default description",
        //        price = 100,
        //        weight = 0.7f,
        //        inventoryWidth = 2,
        //        weaponClass = "knife",
        //        weaponSubClass = "default",
        //        defaultAmmoId = "implicted_combat_knife",
        //        grip = "meleeonehanded",
        //        requiredAmmo = "",
        //        overrideAmmo = "",
        //        bonusAccuracy = 0.1f,
        //        minimumDamage = 12,
        //        maximumDamage = 16,
        //        criticalChance = 1.5f,
        //        criticalDamage = 2f,
        //        firemodes = new List<string>() { "knife_single", "knife_triple" },
        //        magazineCapacity = 0,
        //        reloadDuration = 1,
        //        reloadOneBulletAtATime = false,
        //        maxDurability = 90,
        //        minDurabilityAfterRepair = 0,
        //        unbreakable = false,
        //        repairCategory = "pierce_melee",
        //        isMelee = true,
        //        doesMeleeSplash = false,
        //        canThrow = true,
        //        throwRange = 5,
        //        throwGuaranteedHit = false,
        //        doesThrowPierce = false,
        //        durabilityLossOnThrow = 1,
        //        canMeleeAmputate = true,
        //        amputationOnWound = false,
        //        range = 1,
        //        rangeExtraThrowback = false,
        //        rangeThrowbackChanceBonus = 0,
        //        bonusScatterAngle = 0,
        //        minRandomAmmoCount = 0,
        //        silencerShotChance = 0,
        //        obstaclePierceChanceBonus = 0,
        //        creaturePierceBonus = 0,
        //        woundChanceOnPierce = 0,
        //        defaultGrenadeId = "",
        //        AllowedGrenadeIds = new List<string>(),
        //        isSelfCharge = false,
        //        dotWoundsDamageBonus = 0,
        //        fractureWoundDamageBonus = 0,
        //        painDamageMultiplier = 1f,
        //        critPainDamageMultiplier = 1f,
        //        offSlotCritChance = 0,
        //        minDmgCapBonus = 0,
        //        rampUpValue = 0,
        //        fovLookAngleMult = 1f,
        //        hasHFGOverlay = false,
        //        randomAttackSoundBank = "",
        //        randomDryShotSoundBank = "",
        //        randomFailedAttackSoundBank = "",
        //        randomReloadSoundBank = "",
        //        iconPath = "Images/Knife.png",
        //        smallIconPath = "Images/SmallDagger.png",
        //        shadowOnFloorSpritePath = "folder/filename.extension"
        //    };
        //    return weaponTemplate;
        //}
    }
}