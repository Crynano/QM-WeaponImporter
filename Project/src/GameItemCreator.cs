using MGSC;
using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using static MGSC.Localization;

namespace QM_WeaponImporter
{
    public static class GameItemCreator
    {
        private static Dictionary<string, string> StoredDescriptions = new Dictionary<string, string>();

        public static void Init()
        {
            StoredDescriptions = new Dictionary<string, string>();
        }

        public static bool CreateWeapon(WeaponTemplate userWeapon)
        {
            try
            {
                Logger.WriteToLog($"Creating weapon with ID: {userWeapon.id}");
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
                SetDescriptorProperties(ref myWeapon, userWeapon);
                myWeapon.DefineClassTraits();
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.Source}");
                Logger.FlushAdditive();
                return false;
            }
        }

        public static bool CreateMeleeWeapon(MeleeWeaponTemplate userWeapon)
        {
            try
            {
                Logger.WriteToLog($"Creating MELEE weapon with ID: {userWeapon.id}");
                WeaponRecord myWeapon = new WeaponRecord();
                ConfigureMeleeWeapon(ref myWeapon, userWeapon);
                SetCommonProperties(ref myWeapon, userWeapon);
                SetDescriptorProperties(ref myWeapon, userWeapon);
                myWeapon.DefineClassTraits();
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Melee weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.Source}");
                Logger.FlushAdditive();
                return false;
            }
        }

        public static bool CreateRangedWeapon(RangedWeaponTemplate userWeapon)
        {
            try
            {
                Logger.WriteToLog($"Creating RANGED weapon with ID: {userWeapon.id}");
                WeaponRecord myWeapon = new WeaponRecord();
                ConfigureRangedWeapon(ref myWeapon, userWeapon);
                SetCommonProperties(ref myWeapon, userWeapon);
                SetDescriptorProperties(ref myWeapon, userWeapon);
                myWeapon.DefineClassTraits();
                MGSC.Data.Items.AddRecord(myWeapon.Id, myWeapon);
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Ranged weapon [{userWeapon.id}] couldn't be added.\n{e.Message}\n{e.Source}");
                Logger.FlushAdditive();
                return false;
            }
        }

        private static void SetDescriptorProperties(ref WeaponRecord myWeapon, WeaponTemplate userWeapon)
        {
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
            if (userWeapon.grip == null || userWeapon.grip.Equals(string.Empty))
            {
                myWeaponDescriptor._grip = HandsGrip.BareHands;
            }
            else
            {
                myWeaponDescriptor._grip = StringToEnum<MGSC.HandsGrip>(userWeapon.grip);
            }

            myWeaponDescriptor._hasHFGOverlay = userWeapon.hasHFGOverlay;

            // TODO: Still have to implement
            var foundMuzzles = MonoBehaviour.FindObjectsOfType<Muzzle>();
            List<Muzzle> singleMuzzle = [.. foundMuzzles];
            GameObject muzzleGo = new GameObject("Test Muzzle");
            Muzzle muzzle = muzzleGo.AddComponent<Muzzle>();
            muzzleGo.AddComponent<Light2D>();
            muzzle._additLightIntencityMult = 1f;
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


            // Custom sound bank.
            SoundBank myWeaponSoundBank = new SoundBank();
            //AudioClip ac = new AudioClip("myweapon_default", int lengthSamples, int channels, int frequency, bool stream);
            //myWeaponSoundBank._clips = 
            //[

            //];

            MGSC.Data.Descriptors.TryGetValue("meleeweapons", out DescriptorsCollection meleeWeaponsDescriptors);
            // This gets the army_knife sounds by default
            if ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0] != null)
            {
                myWeaponDescriptor._attackSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._attackSoundBanks;
                myWeaponDescriptor._dryShotSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._dryShotSoundBanks;
                myWeaponDescriptor._failedAttackSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._failedAttackSoundBanks;
                myWeaponDescriptor._reloadSoundBanks = ((WeaponDescriptor)meleeWeaponsDescriptors._descriptors[0])._reloadSoundBanks;
            }

            myWeapon.ContentDescriptor = myWeaponDescriptor;
            Logger.WriteToLog($"Weapon Descriptor for [{userWeapon.id}] has been added successfully!");
        }

        #region Configurators
        private static void SetCommonProperties(ref WeaponRecord myWeapon, WeaponTemplate userWeapon)
        {
            myWeapon.Id = userWeapon.id;
            myWeapon.Price = userWeapon.price;
            myWeapon.Weight = userWeapon.weight;
            myWeapon.InventoryWidthSize = userWeapon.inventoryWidth;
            myWeapon.WeaponClass = StringToEnum<WeaponClass>(userWeapon.weaponClass);
            myWeapon.WeaponSubClass = StringToEnum<WeaponSubClass>(userWeapon.weaponSubClass);
            myWeapon.DefaultAmmoId = userWeapon.defaultAmmoId;
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
            myWeapon.MinDmgCapBonus = userWeapon.minDmgCapBonus;
            myWeapon.RampUpValue = userWeapon.rampUpValue;
            myWeapon.FovLookAngleMult = userWeapon.fovLookAngleMult;
        }

        private static void ConfigureMeleeWeapon(ref WeaponRecord myWeapon, MeleeWeaponTemplate userWeapon)
        {
            myWeapon.IsMelee = userWeapon.isMelee;
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
        }
        #endregion

        #region Utilities

        public static bool AddItemsToFactions(FactionTemplate factionTemplate)
        {
            try
            {
                AddToFactionTable(factionTemplate, ContentDropTableType.Units);
                AddToFactionTable(factionTemplate, ContentDropTableType.Items);
                AddToFactionTable(factionTemplate, ContentDropTableType.TradeItems);
                AddToFactionTable(factionTemplate, ContentDropTableType.RewardItems);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Couldn't add items to faction.\nError: {e.Message}\nSource: {e.Source}");
                return false;
            }
        }

        private static void AddToFactionTable(FactionTemplate factionTemplate, ContentDropTableType tableType)
        {
            // Just add them, how difficult can it be? :P
            // Get the list from Data. Check if it exists
            // Because we don't have any, we have to get the name of the table?
            // Then if exists, search for the difficulty number under the list
            // If it exists, then add the item to the list
            string tableName = "";
            List<Items> selectedTable;
            switch (tableType)
            {
                case ContentDropTableType.Items:
                    selectedTable = factionTemplate.Items;
                    tableName = "items";
                    break;
                case ContentDropTableType.Units:
                    selectedTable = factionTemplate.Units;
                    tableName = "units";
                    break;
                case ContentDropTableType.TradeItems:
                    selectedTable = factionTemplate.TradeItems;
                    tableName = "tradeItems";
                    break;
                case ContentDropTableType.RewardItems:
                    selectedTable = factionTemplate.RewardItems;
                    tableName = "rewardItems";
                    break;
                default:
                    Logger.WriteToLog($"Table {tableType} was not found. Are you sure its correct?");
                    return;
            }
            string itemFactionTableName = factionTemplate.FactionName + "_" + tableName;
            MGSC.Data.FactionDrop._recordsByFactions.TryGetValue(itemFactionTableName, out Dictionary<int, List<ContentDropRecord>> itemFactionTable);
            if (itemFactionTable != null)
            {
                foreach (var itemTable in selectedTable)
                {
                    foreach (var item in itemTable.contentRecords)
                    {
                        itemFactionTable[itemTable.difficulty].Add(item);
                        Logger.WriteToLog($"Adding {item.ContentIds[0]} to the table {itemFactionTableName}");
                    }
                }
            }
            else
            {
                Logger.WriteToLog($"Table [{itemFactionTableName}] not found in the Faction Drops collection.\nCheck if [{factionTemplate.FactionName}] is correct.");
            }
        }

        // writes to the localization db entries as passed
        // entries are denoted type.id.group 
        // ex: item.weapon_id.name , item.weapon_id.shortdesc
        public static void AddLocalization(string type, string group, Dictionary<string, Dictionary<string, string>> localization)
        {
            Dictionary<Lang, Dictionary<string, string>> localizationDb = MGSC.Localization.Instance.db;
            foreach (KeyValuePair<string, Dictionary<string, string>> itemEntry in localization)
            {
                foreach (KeyValuePair<string, string> locals in itemEntry.Value)
                {
                    MGSC.Localization.Lang enumKey;
                    string entryStringId;
                    if (MGSC.Localization.Lang.TryParse(locals.Key, out enumKey))
                    {
                        entryStringId = type + "." + itemEntry.Key + "." + group;
                        if (!localizationDb[enumKey].ContainsKey(entryStringId))
                        {
                            localizationDb[enumKey].Add(entryStringId, locals.Value);
                        }

                    }
                }
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

        #endregion
    }
}