using MGSC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_WeaponImporter
{
    public static class Main
    {
        public static void Init()
        {
            Logger.WriteToLog("Starting Init!");
            Importer.CreateDefaultConfigFile();
            Logger.WriteToLog("Finishing Init!");
            Logger.FlushAdditive();
        }

        private static Dictionary<string, string> StoredDescriptions = new Dictionary<string, string>();
        [Hook(ModHookType.AfterBootstrap)]
        public static void Start(IModContext context)
        {
            try
            {
                Init();
                // We will do a sneaky trick. Every time the language changes, we just write it in the original language.
                // Until we can improve this of course...
                StoredDescriptions = new Dictionary<string, string>();
                Localization.Instance.OnLangChanged += Instance_OnLangChanged;
                ImportWeapons();
                Logger.FlushAdditive();
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"An error occurred during init: \n{e.Message}\nfrom {e.Source}");
                Logger.Flush();
            }
        }

        private static void ImportWeapons()
        {
            // Call importer
            // For each weapon in the list, call create weapon.
            var allWeapons = Importer.ImportUserWeapons();
            if (allWeapons == null)
            {
                Logger.WriteToLog($"Default Weapons is null. Internal Error.");
                return;
            }

            foreach (var weapon in allWeapons.Weapons)
            {
                if (!CreateNamedWeapon(weapon))
                {
                    Logger.WriteToLog($"Weapon {weapon.id} could not be loaded");
                }
            }
        }

        public static bool CreateNamedWeapon(WeaponTemplate userWeapon)
        {
            try
            {
                Logger.WriteToLog($"Starting CreateNamedWeapon for: {userWeapon.id}");

                // My goal here is to expose all the parameters so people can see what value is each variable
                WeaponRecord myWeapon = new WeaponRecord();
                myWeapon.Id = userWeapon.id;
                myWeapon.Price = userWeapon.price;
                myWeapon.Weight = userWeapon.weight;
                myWeapon.InventoryWidthSize = userWeapon.inventoryWidth;
                myWeapon.WeaponClass = StringToEnum<WeaponClass>(userWeapon.weaponClass);
                myWeapon.WeaponSubClass = StringToEnum<WeaponSubClass>(userWeapon.weaponSubClass);
                myWeapon.DefaultAmmoId = userWeapon.defaultAmmoId;
                myWeapon.IsMelee = userWeapon.isMelee;
                myWeapon.Damage = new DmgInfo()
                {
                    minDmg = userWeapon.minimumDamage,
                    maxDmg = userWeapon.maximumDamage,
                    critChance = userWeapon.criticalChance,
                    critDmg = userWeapon.criticalDamage
                };
                myWeapon.Firemodes = userWeapon.firemodes;
                myWeapon.BonusAccuracy = userWeapon.bonusAccuracy;
                myWeapon.MagazineCapacity = userWeapon.magazineCapacity;
                myWeapon.ReloadDuration = userWeapon.reloadDuration;
                myWeapon.ReloadOneClip = userWeapon.reloadOneBulletAtATime;
                myWeapon.DurabilityLossOnThrow = userWeapon.durabilityLossOnThrow;
                myWeapon.ThrowRange = userWeapon.throwRange;
                myWeapon.MeleeCanAmputate = userWeapon.canMeleeAmputate;
                myWeapon.MaxDurability = userWeapon.maxDurability;
                myWeapon.MinDurabilityAfterRepair = userWeapon.minDurabilityAfterRepair;
                myWeapon.Unbreakable = userWeapon.unbreakable;
                myWeapon.RepairCategory = userWeapon.repairCategory;
                myWeapon.RequiredAmmo = userWeapon.requiredAmmo;
                myWeapon.OverrideAmmo = userWeapon.overrideAmmo;
                myWeapon.SplashMeleeAttack = userWeapon.doesMeleeSplash;
                myWeapon.CanThrow = userWeapon.canThrow;
                myWeapon.ThrowAutoHit = userWeapon.throwGuaranteedHit;
                myWeapon.ThrowPierce = userWeapon.doesThrowPierce;
                myWeapon.DoAmputationOnWound = userWeapon.amputationOnWound;
                myWeapon.Range = userWeapon.range;
                myWeapon.RangeExtraThrowback = userWeapon.rangeExtraThrowback;
                myWeapon.RangeThrowbackChanceBonus = userWeapon.rangeThrowbackChanceBonus;
                myWeapon.BonusScatterAngle = userWeapon.bonusScatterAngle;
                myWeapon.MinRandomAmmoCount = userWeapon.minRandomAmmoCount;
                myWeapon.SilencerShotChance = userWeapon.silencerShotChance;
                myWeapon.ObstaclePierceChanceBonus = userWeapon.obstaclePierceChanceBonus;
                myWeapon.CreaturePierceBonus = userWeapon.creaturePierceBonus;
                myWeapon.WoundChanceOnPierce = userWeapon.woundChanceOnPierce;
                myWeapon.DefaultGrenadeId = userWeapon.defaultGrenadeId;
                myWeapon.AllowedGrenadeIds = userWeapon.AllowedGrenadeIds;
                myWeapon.IsSelfCharge = userWeapon.isSelfCharge;
                myWeapon.DotWoundsDmgBonus = userWeapon.dotWoundsDamageBonus;
                myWeapon.FractureWoundDmgBonus = userWeapon.fractureWoundDamageBonus;
                myWeapon.PainDamageMult = userWeapon.painDamageMultiplier;
                myWeapon.CritPainDamageMult = userWeapon.critPainDamageMultiplier;
                myWeapon.OffSlotCritChance = userWeapon.offSlotCritChance;
                myWeapon.MinDmgCapBonus = userWeapon.minDmgCapBonus;
                myWeapon.RampUpValue = userWeapon.rampUpValue;
                myWeapon.FovLookAngleMult = userWeapon.fovLookAngleMult;
                myWeapon.DefineClassTraits();

                WeaponDescriptor myWeaponDescriptor = ScriptableObject.CreateInstance("WeaponDescriptor") as WeaponDescriptor;
                myWeaponDescriptor._overridenRenderId = userWeapon.id;

                Sprite itemSprite = Importer.LoadNewSprite(userWeapon.iconPath);
                if (itemSprite != null)
                    myWeaponDescriptor._icon = itemSprite;

                Sprite smallIconSprite = Importer.LoadNewSprite(userWeapon.smallIconPath);
                if (smallIconSprite != null)
                    myWeaponDescriptor._smallIcon = smallIconSprite;

                Sprite shadowSprite = Importer.LoadNewSprite(userWeapon.shadowOnFloorSpritePath);
                if (shadowSprite != null)
                    myWeaponDescriptor._shadow = shadowSprite;

                // If grip is null then a bunch of problems will arise.
                if (userWeapon.grip == null || userWeapon.grip == string.Empty)
                {
                    myWeaponDescriptor._grip = HandsGrip.BareHands;
                }
                else
                {
                    myWeaponDescriptor._grip = StringToEnum<MGSC.HandsGrip>(userWeapon.grip);
                }

                myWeaponDescriptor._hasHFGOverlay = userWeapon.hasHFGOverlay || false;

                // TODO: Still have to implement
                //myWeaponDescriptor._muzzles = new List<Muzzle>().ToArray();
                //myWeaponDescriptor._overrideBullet = userWeapon.overridenBulletPrefab;
                // Let's set some defaults
                MGSC.Data.Descriptors.TryGetValue("meleeweapons", out DescriptorsCollection meleeWeaponsDescriptors);

                // Custom sound bank.
                SoundBank myWeaponSoundBank = new SoundBank();
                //AudioClip ac = new AudioClip("myweapon_default", int lengthSamples, int channels, int frequency, bool stream);
                //myWeaponSoundBank._clips = 
                //[

                //];

                // This gets the army_knife sounds by default
                if ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0] != null)
                {
                    myWeaponDescriptor._attackSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._attackSoundBanks;
                    myWeaponDescriptor._dryShotSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._dryShotSoundBanks;
                    myWeaponDescriptor._failedAttackSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._failedAttackSoundBanks;
                    myWeaponDescriptor._reloadSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._reloadSoundBanks;
                }

                myWeapon.ContentDescriptor = myWeaponDescriptor;
                AddLocalization(userWeapon);
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);

                Logger.WriteToLog($"Weapon [{myWeapon.Id}] has been added successfully!");
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.Source}");
                Logger.FlushAdditive();
                return false;
            }
        }

        private static void AddLocalization(WeaponTemplate userWeapon)
        {
            // TODO Consider other languages. And add it to the JSON somehow
            Localization.Instance.currentDict.Add($"item.{userWeapon.id}.name", userWeapon.name);
            Localization.Instance.currentDict.Add($"item.{userWeapon.id}.shortdesc", userWeapon.description);
            StoredDescriptions.Add($"item.{userWeapon.id}.name", userWeapon.name);
            StoredDescriptions.Add($"item.{userWeapon.id}.shortdesc", userWeapon.description);
        }

        private static void Instance_OnLangChanged()
        {
            // Here we add to the current database (if not already in) our text!
            // Or maybe not yet?
            foreach (var entry in StoredDescriptions)
            {
                if (!Localization.Instance.currentDict.ContainsKey(entry.Key))
                    Localization.Instance.currentDict.Add(entry.Key, entry.Value);
            }
        }

        private static T StringToEnum<T>(string type) where T : Enum
        {
            foreach (T wClass in Enum.GetValues(typeof(T)))
            {
                if (wClass.ToString().ToLower().Contains(type.ToLower())) return wClass;
            }
            return (T)Enum.ToObject(typeof(T), 0);
        }
    }
}