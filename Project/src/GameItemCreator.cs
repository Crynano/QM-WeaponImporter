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
                Logger.LogInfo($"Creating weapon with ID: {userWeapon.id}");
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
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
                Logger.LogInfo($"Weapon [{myWeapon.Id}] LOADED");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.StackTrace}");
                Logger.FlushAdditive();
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
            myWeaponDescriptor._icon = Importer.LoadNewSprite(weaponDescriptor.iconSpritePath);
            myWeaponDescriptor._smallIcon = Importer.LoadCenteredSprite(weaponDescriptor.smallIconSpritePath);
            myWeaponDescriptor._shadow = Importer.LoadCenteredSprite(weaponDescriptor.shadowOnFloorSpritePath);

            // If grip is null then a bunch of problems will arise.
            myWeaponDescriptor._grip = userWeapon.grip;

            // Object prefab with muzzle
            myWeaponDescriptor._prefab = Importer.LoadFileFromBundle<GameObject>(weaponDescriptor.bundlePath, weaponDescriptor.prefabName);
            myWeaponDescriptor._texture = Importer.LoadFileFromBundle<Texture>(weaponDescriptor.bundlePath, weaponDescriptor.textureName);

            // HFG Overlay?
            myWeaponDescriptor._hasHFGOverlay = userWeapon.hasHFGOverlay;
            bool muzzleApplied = false;
            if (!string.IsNullOrEmpty(weaponDescriptor.baseItemId))
            {
                if (Data.Items._records.ContainsKey(weaponDescriptor.baseItemId))
                {
                    var overridePropertiesWeapon = MGSC.Data.Items.GetSimpleRecord<WeaponRecord>(weaponDescriptor.baseItemId);
                    if (overridePropertiesWeapon != null)
                    {
                        Logger.LogInfo($"Applying on {myWeapon.Id} the muzzle from {weaponDescriptor.baseItemId}");
                        WeaponDescriptor overrideWeaponDescriptor = ((WeaponDescriptor)overridePropertiesWeapon.ContentDescriptor);
                        myWeaponDescriptor._muzzles = overrideWeaponDescriptor._muzzles;
                        muzzleApplied = true;
                    }
                }
                else
                {
                    Logger.LogWarning($"Item with base ID not found: {weaponDescriptor.baseItemId}");
                }
            }
            // If no specified or failed. Then apply default
            if (!muzzleApplied)
            {
                Logger.LogInfo($"Muzzle wasn't herited, applying by default");
                GameObject muzzleGo = new GameObject($"Weapon [{userWeapon.id}] Muzzle");
                muzzleGo.transform.parent = myWeaponDescriptor._prefab.transform;
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
                         time = .2f,
                         value = 0.5f,
                     }
                    ],

                };
                myWeaponDescriptor._muzzles =
                [
                    muzzle
                ];
            }

            //BulletTemplate userBullet = Importer.Load<BulletTemplate>(userWeapon.bulletAssetPath);
            //List<Sprite> facadeDecalsSprites = new List<Sprite>();
            //foreach (string spritePath in userBullet.facadeDecals)
            //{
            //    facadeDecalsSprites.Add(Importer.LoadNewSprite(spritePath, 100f));
            //}
            // bullets have types. Now we are a bit fucked.
            // BUT WAIT, WHAT IF I WANT TO USE A GAME BULLET? FUCK!
            // Filter by type.
            // I believe override bullet must not be set, it works through ammo description
            //myWeaponDescriptor._overrideBullet = new CommonBullet()
            //{
            //    _bulletSpeed = userBullet.bulletSpeed,
            //    _makeBloodDecals = userBullet.makeBloodDecals,
            //    _putShotDecalsOnWalls = userBullet.putShotDecalsOnWalls,
            //    _putBulletShellsOnFloor = userBullet.putBulletShellsOnFloor,
            //    _rotateBulletInShotDir = userBullet.rotateBulletInShotDir,
            //    _shakeDuration = userBullet.shakeDuration,
            //    _shakeStrength = userBullet.shakeStrength,
            //    _facadeDecals = facadeDecalsSprites.ToArray(),
            //    //_gibsController = null;
            //    _putDecals = userBullet.putDecals
            //};

            try
            {
                SetSounds(ref myWeaponDescriptor._attackSoundBanks, weaponDescriptor.shootSoundPath, weaponDescriptor.baseItemId, 0);
                SetSounds(ref myWeaponDescriptor._dryShotSoundBanks, weaponDescriptor.dryShotSoundPath, weaponDescriptor.baseItemId, 1);
                SetSounds(ref myWeaponDescriptor._failedAttackSoundBanks, weaponDescriptor.failedAttackSoundPath, weaponDescriptor.baseItemId, 2);
                SetSounds(ref myWeaponDescriptor._reloadSoundBanks, weaponDescriptor.reloadSoundPath, weaponDescriptor.baseItemId, 3);
            }
            catch (Exception e)
            {
                Logger.LogError($"Trying to add sounds but: {e.Message}\n{e.StackTrace}");
            }

            myWeapon.ContentDescriptor = myWeaponDescriptor;
            Logger.LogInfo($"Weapon Descriptor for [{userWeapon.id}] has been added successfully!");
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
            myWeapon.DoAmputationOnWound = userWeapon.amputationOnWound;
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
                Logger.LogInfo($"Adding rewards to {factionRewardTable.TableName} with {factionRewardTable.contentRecords.Count} items!");
                foreach (var rewardEntry in factionRewardTable.contentRecords)
                {
                    // Devs really pulled an easy on us huh.
                    Logger.LogInfo($"Adding {factionRewardTable.TableName}");
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

        private static void SetSounds(ref SoundBank[] soundBank, string soundPath, string baseItemId, int category)
        {
            if (soundBank == null)
            {
                soundBank = new SoundBank[1];
                soundBank[0] = new SoundBank();
                soundBank[0]._clips = new AudioClip[1];
            }

            AudioClip audioClip = Importer.ImportAudio(soundPath);
            if (audioClip == null)
            {
                soundBank = GetAudiosFromExistingWeapons(baseItemId, category);
                return;
            }
            else
            {
                soundBank[0]._clips[0] = audioClip;
            }
        }

        private static SoundBank[] GetAudiosFromExistingWeapons(string id, int category)
        {
            WeaponDescriptor selectedWeaponDesc;
            if (Data.Items._records.ContainsKey(id))
            {
                var referenceWeapon = MGSC.Data.Items.GetSimpleRecord<WeaponRecord>(id);
                selectedWeaponDesc = referenceWeapon.ContentDescriptor as WeaponDescriptor;

            }
            else
            {
                Data.Descriptors.TryGetValue("rangeweapons", out DescriptorsCollection rngWps);
                selectedWeaponDesc = rngWps._descriptors[0] as WeaponDescriptor;
            }
            switch (category)
            {
                case 0: return selectedWeaponDesc._attackSoundBanks;
                case 1: return selectedWeaponDesc._dryShotSoundBanks;
                case 2: return selectedWeaponDesc._failedAttackSoundBanks;
                case 3: return selectedWeaponDesc._reloadSoundBanks;
                default: return new SoundBank[] { };
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