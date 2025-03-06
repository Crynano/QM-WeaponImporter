using MGSC;
using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_WeaponImporter
{
    internal static class GameItemCreator
    {
        public static bool CreateWeapon(WeaponTemplate userWeapon, CustomItemContentDescriptor weaponDescriptor)
        {
            try
            {
                //Logger.LogInfo($"Creating weapon with ID: {userWeapon.id}");
                WeaponRecord myWeapon = new WeaponRecord();
                if (userWeapon.GetType() == typeof(MeleeWeaponTemplate))
                {
                    // Melee
                    ConfigureMeleeWeapon(ref myWeapon, userWeapon as MeleeWeaponTemplate);
                }
                else if (userWeapon.GetType() == typeof(RangedWeaponTemplate))
                {
                    // Ranged
                    ConfigureRangedWeapon(ref myWeapon, userWeapon as RangedWeaponTemplate);
                }
                SetCommonProperties(ref myWeapon, userWeapon);
                SetDescriptorProperties(ref myWeapon, userWeapon, weaponDescriptor);
                //myWeapon.DefineClassTraits();
                try
                {
                    if (MGSC.Data.Items.GetSimpleRecord<BasePickupItemRecord>(myWeapon.Id) != null)
                    {
                        // If a weapon with that ID, by any case, is already registered. OVERRIDE IT.
                        // In the end this will ensure correctness of mods over other ones.
                        // Also creators must ensure IDs are unique.
                        MGSC.Data.Items.RemoveRecord(myWeapon.Id);
                        Logger.LogWarning($"An item with ID: [{myWeapon.Id}] was OVERRIDEN!!!");
                    }
                }
                catch (Exception ex) { }
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
                Logger.LogInfo($"Loaded successfully.");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"Couldn't be added.\n{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        //public static bool CreateMeleeWeapon(MeleeWeaponTemplate userWeapon, CustomItemContentDescriptor weaponDescriptor)
        //{
        //    try
        //    {
        //        Logger.WriteToLog($"Creating MELEE weapon with ID: {userWeapon.id}");
        //        WeaponRecord myWeapon = new WeaponRecord();
        //        ConfigureMeleeWeapon(ref myWeapon, userWeapon);
        //        SetCommonProperties(ref myWeapon, userWeapon);
        //        SetDescriptorProperties(ref myWeapon, userWeapon);
        //        //myWeapon.DefineClassTraits();
        //        MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.WriteToLog($"Melee weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.InnerException}", Logger.LogType.Error);
        //        Logger.FlushAdditive();
        //        return false;
        //    }
        //}

        //public static bool CreateRangedWeapon(RangedWeaponTemplate userWeapon, CustomItemContentDescriptor weaponDescriptor)
        //{
        //    try
        //    {
        //        Logger.WriteToLog($"Creating RANGED weapon with ID: {userWeapon.id}");
        //        WeaponRecord myWeapon = new WeaponRecord();
        //        ConfigureRangedWeapon(ref myWeapon, userWeapon);
        //        SetCommonProperties(ref myWeapon, userWeapon);
        //        SetDescriptorProperties(ref myWeapon, userWeapon);
        //        //myWeapon.DefineClassTraits();
        //        MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.WriteToLog($"Ranged weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.InnerException}", Logger.LogType.Error);
        //        Logger.FlushAdditive();
        //        return false;
        //    }
        //}

        private static void SetDescriptorProperties(ref WeaponRecord myWeapon, WeaponTemplate userWeapon, CustomItemContentDescriptor weaponDescriptor)
        {
            WeaponDescriptor myWeaponDescriptor = ScriptableObject.CreateInstance("WeaponDescriptor") as WeaponDescriptor;
            // Get the loaded descriptor for the weapon?

            myWeaponDescriptor._overridenRenderId = userWeapon.id;

            // Icons
            // Added the ability to load resources from another weapon.
            myWeaponDescriptor._icon =
                Importer.LoadNewSprite(weaponDescriptor.iconSpritePath)
                ?? GetIconFromExistingWeapon(weaponDescriptor.iconSpritePath);

            myWeaponDescriptor._smallIcon =
                Importer.LoadCenteredSprite(weaponDescriptor.smallIconSpritePath)
                ?? GetSmallIconFromExistingWeapon(weaponDescriptor.smallIconSpritePath);

            myWeaponDescriptor._shadow =
                Importer.LoadCenteredSprite(weaponDescriptor.shadowOnFloorSpritePath)
                ?? GetShadowFromExistingWeapon(weaponDescriptor.shadowOnFloorSpritePath);

            // If grip is null then a bunch of problems will arise.
            myWeaponDescriptor._grip = userWeapon.grip;

            // Simple properties first
            myWeaponDescriptor._hasHFGOverlay = userWeapon.hasHFGOverlay;

            // Object prefab with muzzle
            // Let's do it this way. If prefab has a valid ingame id, then load the prefab from the weapon. Otherwise load from file.
            // Do that for the following properties

            myWeaponDescriptor._prefab =
                Importer.LoadFileFromBundle<GameObject>(weaponDescriptor.bundlePath, weaponDescriptor.prefabName)
                ?? GetPrefabFromExistingWeapon(weaponDescriptor.prefabName);

            myWeaponDescriptor._texture =
                Importer.LoadFileFromBundle<Texture>(weaponDescriptor.bundlePath, weaponDescriptor.textureName)
                ?? GetTextureFromExistingWeapon(weaponDescriptor.textureName);

            myWeaponDescriptor._overrideBullet =
                Importer.LoadFileFromBundle<CommonBullet>(weaponDescriptor.bundlePath, weaponDescriptor.bulletName)
                ?? GetBulletFromExistingWeapon(weaponDescriptor.bulletName);

            myWeaponDescriptor._muzzles = new Muzzle[1];
            myWeaponDescriptor._muzzles[0] =
               Importer.LoadFileFromBundle<Muzzle>(weaponDescriptor.bundlePath, weaponDescriptor.muzzleName)
               ?? GetMuzzleFromExistingWeapon(weaponDescriptor.muzzleName)
               ?? LoadDefaultMuzzle();

            try
            {
                SetSounds(ref myWeaponDescriptor._attackSoundBanks, weaponDescriptor.shootSoundPath, 0);
                SetSounds(ref myWeaponDescriptor._dryShotSoundBanks, weaponDescriptor.dryShotSoundPath, 1);
                SetSounds(ref myWeaponDescriptor._failedAttackSoundBanks, weaponDescriptor.failedAttackSoundPath, 2);
                SetSounds(ref myWeaponDescriptor._reloadSoundBanks, weaponDescriptor.reloadSoundPath, 3);
            }
            catch (Exception e)
            {
                Logger.LogError($"Trying to add sounds but: {e.Message}\n{e.StackTrace}");
            }

            myWeapon.ContentDescriptor = myWeaponDescriptor;
            Logger.LogInfo($"Descriptor added successfully.");
        }

        #region Configurators
        private static void SetCommonProperties(ref WeaponRecord myWeapon, WeaponTemplate userWeapon)
        {
            // TODO FIX THIS
            myWeapon.Id = userWeapon.id;
            myWeapon.Price = userWeapon.price;
            myWeapon.Weight = userWeapon.weight;
            myWeapon.InventoryWidthSize = userWeapon.inventoryWidthSize;
            myWeapon.TechLevel = userWeapon.techLevel;
            myWeapon.Categories = userWeapon.categories;
            myWeapon.IsImplicit = userWeapon.isImplicit;
            myWeapon.ItemClass = userWeapon.itemClass;
            myWeapon.WeaponClass = userWeapon.weaponClass;
            myWeapon.WeaponSubClass = userWeapon.weaponSubClass;
            myWeapon.DefaultAmmoId = userWeapon.defaultAmmoId;
            // Let's throw a warning to the user just in case.
            if (!MGSC.Data.Items._records.ContainsKey(userWeapon.defaultAmmoId))
            {
                // Does not contain value
                Logger.LogError($"Weapon won't load, ammunition does not exist.");
                throw new NullReferenceException($"Ammunition with {userWeapon.defaultAmmoId} does not exist in the game data");
            }

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
            myWeapon.MaxDurability = userWeapon.maxDurability;
            myWeapon.MinDurabilityAfterRepair = userWeapon.minDurabilityAfterRepair;
            myWeapon.Unbreakable = userWeapon.unbreakable;
            myWeapon.RepairCategory = userWeapon.repairCategory;
            myWeapon.RequiredAmmo = userWeapon.requiredAmmo;
            myWeapon.OverrideAmmo = userWeapon.overrideAmmo;
            myWeapon.Range = userWeapon.range;
            myWeapon.IsSelfCharge = userWeapon.isSelfCharge;
            myWeapon.DotWoundsDmgBonus = userWeapon.dotWoundsDamageBonus;
            myWeapon.FractureWoundDmgBonus = userWeapon.fractureWoundDamageBonus;
            myWeapon.PainDamageMult = userWeapon.painDamageMultiplier;
            myWeapon.CritPainDamageMult = userWeapon.critPainDamageMultiplier;
            myWeapon.OffSlotCritChance = userWeapon.offSlotCritChance;
            myWeapon.FovLookAngleMult = userWeapon.fovLookAngleMult;
            myWeapon.DoAmputationOnWound = userWeapon.amputationOnWound;
            myWeapon.ObstaclePierceChanceBonus = userWeapon.obstaclePierceChanceBonus;
            myWeapon.CreaturePierceBonus = userWeapon.creaturePierceBonus;
            myWeapon.WoundChanceOnPierce = userWeapon.woundChanceOnPierce;
            myWeapon.ArmorPenetration = userWeapon.armorPenetration;
            myWeapon.IsImplicit = userWeapon.isImplicit;
        }

        private static void ConfigureMeleeWeapon(ref WeaponRecord myWeapon, MeleeWeaponTemplate userWeapon)
        {
            myWeapon.IsMelee = true;
            myWeapon.DurabilityLossOnThrow = userWeapon.durabilityLossOnThrow;
            myWeapon.ThrowRange = userWeapon.throwRange;
            myWeapon.MeleeCanAmputate = userWeapon.canMeleeAmputate;
            myWeapon.SplashMeleeAttack = userWeapon.doesMeleeSplash;
            myWeapon.CanThrow = userWeapon.canThrow;
            myWeapon.ThrowAutoHit = userWeapon.throwGuaranteedHit;
            myWeapon.ThrowPierce = userWeapon.doesThrowPierce;
        }

        private static void ConfigureRangedWeapon(ref WeaponRecord myWeapon, RangedWeaponTemplate userWeapon)
        {
            myWeapon.IsMelee = false;
            myWeapon.ArmorPenetration = userWeapon.armorPenetration;
            myWeapon.RangeExtraThrowback = userWeapon.rangeExtraThrowback;
            myWeapon.RangeThrowbackChanceBonus = userWeapon.rangeThrowbackChanceBonus;
            myWeapon.BonusScatterAngle = userWeapon.bonusScatterAngle;
            myWeapon.MinRandomAmmoCount = userWeapon.minRandomAmmoCount;
            myWeapon.SilencerShotChance = userWeapon.silencerShotChance;
            myWeapon.DefaultGrenadeId = userWeapon.defaultGrenadeId;
            myWeapon.AllowedGrenadeIds = userWeapon.AllowedGrenadeIds;
            myWeapon.RampUpValue = userWeapon.rampUpValue;
        }
        #endregion

        #region Utilities

        public static void AddItemsToFactions(FactionTemplate factionTemplate)
        {
            AddToFactionTable(factionTemplate);
        }

        private static void AddToFactionTable(FactionTemplate factionTemplate)
        {
            var selectedTable = factionTemplate.FactionRewardList;
            foreach (var factionRewardTable in selectedTable)
            {
                foreach (var rewardEntry in factionRewardTable.contentRecords)
                {
                    string ids = string.Concat(rewardEntry.ContentIds);
                    Logger.LogInfo($"Adding [{ids}] to {factionRewardTable.FactionName} faction table.");
                    if (rewardEntry.ContentIds.Count > 0 && Data.Items._records.ContainsKey(rewardEntry.ContentIds[0]))
                        Data.FactionDrop.AddRecord(factionRewardTable.TableName, rewardEntry);
                }
            }
        }

        // writes to the localization db entries as passed
        // entries are denoted type.id.group 
        // ex: item.weapon_id.name , item.weapon_id.shortdesc
        public static void AddLocalization(string type, string group, Dictionary<string, Dictionary<string, string>> localization)
        {
            Dictionary<MGSC.Localization.Lang, Dictionary<string, string>> localizationDb = Localization.Instance.db;
            foreach (KeyValuePair<string, Dictionary<string, string>> itemEntry in localization)
            {
                foreach (KeyValuePair<string, string> locals in itemEntry.Value)
                {
                    MGSC.Localization.Lang enumKey;
                    string entryStringId;
                    if (MGSC.Localization.Lang.TryParse(locals.Key, out enumKey))
                    {
                        entryStringId = $"{type}.{itemEntry.Key}.{group}";
                        if (!localizationDb[enumKey].ContainsKey(entryStringId))
                        {
                            localizationDb[enumKey].Add(entryStringId, locals.Value);
                        }

                    }
                }
            }
        }

        private static void SetSounds(ref SoundBank[] soundBank, string soundPath, int category)
        {
            if (soundBank == null)
            {
                soundBank = new SoundBank[1];
                soundBank[0] = ScriptableObject.CreateInstance(typeof(SoundBank)) as SoundBank;
                soundBank[0]._clips = new AudioClip[1];
            }

            var existingWeaponAudio = GetAudiosFromExistingWeapons(soundPath, category);
            if (existingWeaponAudio != null)
            {
                soundBank = existingWeaponAudio;
                return;
            }

            AudioClip audioClip = Importer.ImportAudio(soundPath);
            if (audioClip != null)
            {
                soundBank[0]._clips[0] = audioClip;
            }
        }

        private static SoundBank[] GetAudiosFromExistingWeapons(string id, int category)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            if (selectedDescriptor == null) return null;
            switch (category)
            {
                case 0: return selectedDescriptor._attackSoundBanks;
                case 1: return selectedDescriptor._dryShotSoundBanks;
                case 2: return selectedDescriptor._failedAttackSoundBanks;
                case 3: return selectedDescriptor._reloadSoundBanks;
                default: return null;
            }
        }

        private static GameObject GetPrefabFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            return selectedDescriptor is not null ? selectedDescriptor.Prefab : null;
        }

        private static Texture GetTextureFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            return selectedDescriptor is not null ? selectedDescriptor._texture : null;
        }

        private static Muzzle GetMuzzleFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id, true);
            return selectedDescriptor is not null && selectedDescriptor._muzzles.Length > 0 ? selectedDescriptor._muzzles[0] : null;
        }

        private static Muzzle LoadDefaultMuzzle()
        {
            GameObject muzzleGo = new GameObject($"Muzzle");
            Muzzle muzzle = muzzleGo.AddComponent<Muzzle>();
            muzzle._additLightIntencityMult = .5f;
            muzzle._muzzleIntensityCurve = new AnimationCurve()
            {
                keys =
                [
                    new Keyframe()
                    {
                        time = 0,
                        value = 0,
                    },
                    new Keyframe()
                    {
                        time = .05f,
                        value = 0.5f,
                    },
                    new Keyframe()
                    {
                        time = .1f,
                        value = 0,
                    },
                ],

            };
            return muzzle;
        }

        private static CommonBullet GetBulletFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id, true);
            return selectedDescriptor is not null ? selectedDescriptor._overrideBullet : null;
        }

        private static Sprite GetIconFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            return selectedDescriptor is not null ? selectedDescriptor.Icon : null;
        }

        private static Sprite GetSmallIconFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            return selectedDescriptor is not null ? selectedDescriptor.SmallIcon : null;
        }

        private static Sprite GetShadowFromExistingWeapon(string id)
        {
            WeaponDescriptor selectedDescriptor = GetExistingWeaponDescriptor(id);
            return selectedDescriptor is not null ? selectedDescriptor.ShadowOnFloor : null;
        }

        private static WeaponDescriptor GetExistingWeaponDescriptor(string id, bool getDefault = true)
        {
            WeaponDescriptor selectedDescriptor = null;
            if (Data.Items._records.ContainsKey(id))
            {
                var referenceWeapon = MGSC.Data.Items.GetSimpleRecord<WeaponRecord>(id);
                selectedDescriptor = referenceWeapon.ContentDescriptor as WeaponDescriptor;
            }
            else if (getDefault)
            {
                Data.Descriptors.TryGetValue("rangeweapons", out DescriptorsCollection rngWps);
                Logger.LogWarning($"Item with ID: <{id}> not found in-game. Using <{rngWps._ids[0]}> as default.");
                return rngWps._descriptors[0] as WeaponDescriptor;
            }
            return selectedDescriptor;
        }

        private static T StringToEnum<T>(string type) where T : Enum
        {
            foreach (T wClass in Enum.GetValues(typeof(T)))
            {
                if (wClass.ToString().ToLower().Contains(type.ToLower())) return wClass;
            }
            return (T)Enum.ToObject(typeof(T), 0);
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            var type = original.GetType();
            var copy = destination.AddComponent(type);
            var fields = type.GetFields();
            foreach (var field in fields) field.SetValue(copy, field.GetValue(original));
            return copy as T;
        }

        #endregion
    }
}