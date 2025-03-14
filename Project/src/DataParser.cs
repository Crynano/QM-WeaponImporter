using MGSC;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace QM_WeaponImporter
{
    // The idea of the ConfigManager is that imports the GLOBAL config first. Then each of the paths points to the folder to the configurable items
    // Also, calls to the function should pass the class that specifies every path. So someone using it as an API can send an instance of the class
    // and the config manager uses that to load paths. Otherwise if no file has been sent just load by default.
    // BUt if multiple mods are loading stuff at the same time, they should have the file or class and say, load these weapons please.
    // The class should include the root folder. Maybe a simple ../ would work but let's check.
    // We can assume it wont be needed but yeah.
    internal static class DataParser
    {
        public static string rootFolder;

        private static List<IConfigParser> Parsers = new List<IConfigParser>();

        // How about we load descriptors first?
        private static List<CustomItemContentDescriptor> itemDescriptors = new List<CustomItemContentDescriptor>();

        private static void LoadDescriptors(ConfigTemplate userConfig)
        {
            //Logger.LogInfo($"Loading descriptors");
            Parsers.Add(new TemplateParser<CustomItemContentDescriptor>("descriptorsPath", itemDescriptors.Add));
            KeyValuePair<string, string> descriptorsEntry = new KeyValuePair<string, string>("descriptorsPath", userConfig.descriptorsPath);
            if (!ParseFile(descriptorsEntry))
            {
                // If it doesn't work, interrupt
                Logger.LogError($"Interrupting Mod Load: Descriptors Folder Path not found in {descriptorsEntry.Value}.\nPlease add them in the {Importer.GlobalConfigName} file.");
                throw new NullReferenceException($"Critical error: Descriptors Folder Path not found in {descriptorsEntry.Value}.\nPlease add them in the {Importer.GlobalConfigName} file.");
            }
        }

        private static void LoadDefaultParsers()
        {
            // We can just copy the code they used xD.
            // TODO -- port this to the ImportParser
            // ------- eliminate MeleeWeaponTemplate
            Parsers.Add(new NullableRecordParser<AmmoRecord>("ammo", delegate (AmmoRecord ammoItem)
            {
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(ammoItem.Id);
                Data.Descriptors.TryGetValue("ammo", out DescriptorsCollection ammoDescriptors);
                AmmoDescriptor baseAmmo = ammoDescriptors.GetDescriptor(customItemDescriptor.baseItemId) as AmmoDescriptor;
                ItemContentDescriptor originalDescriptor = customItemDescriptor.GetOriginal<ItemContentDescriptor>();
                AmmoDescriptor ammoContentDescriptor = new AmmoDescriptor();
                if (baseAmmo != null)
                {
                    ammoContentDescriptor._bullet = baseAmmo._bullet;
                    ammoContentDescriptor._meleeMakeBloodDecal = baseAmmo._meleeMakeBloodDecal;
                    ammoContentDescriptor._gibs = baseAmmo._gibs;
                }
                // Use base icons if user provided no icons.
                ammoContentDescriptor._icon = originalDescriptor._icon == null && baseAmmo != null ? baseAmmo._icon : originalDescriptor._icon;
                ammoContentDescriptor._smallIcon = originalDescriptor._smallIcon == null && baseAmmo != null ? baseAmmo._smallIcon : originalDescriptor._smallIcon;
                ammoContentDescriptor._shadow = originalDescriptor._shadow == null && baseAmmo != null ? baseAmmo._shadow : originalDescriptor._icon;
                ammoItem.ContentDescriptor = ammoContentDescriptor;
                AddItemToGame(ammoItem);
            }));
            Parsers.Add(new TemplateParser<MeleeWeaponTemplate>("meleeweapons", delegate (MeleeWeaponTemplate weaponTemplate)
            {
                Logger.SetContext(weaponTemplate.id);
                GameItemCreator.CreateWeapon(weaponTemplate, GetDescriptor(weaponTemplate.id));
            }));
            // TODO -- port this to the ImportParser
            // ------- eliminate RangedWeaponTemplate
            Parsers.Add(new TemplateParser<RangedWeaponTemplate>("rangedweapons", delegate (RangedWeaponTemplate weaponTemplate)
            {
                Logger.SetContext(weaponTemplate.id);
                GameItemCreator.CreateWeapon(weaponTemplate, GetDescriptor(weaponTemplate.id));
            }));
            Parsers.Add(new ImportParser<ItemTransformationRecord>("itemtransforms", delegate (ItemTransformationRecord itemTransformRecord)
            {
                AddItemTransform(itemTransformRecord);
                //MGSC.Data.ItemTransformation.AddRecord(itemTransformRecord.Id, itemTransformRecord);
            }));
            Parsers.Add(new ImportParser<ItemProduceReceipt>("itemreceipts", delegate (ItemProduceReceipt itemProduceReceiptRecord)
            {
                AddProduceReceipt(itemProduceReceiptRecord);
                //MGSC.Data.ProduceReceipts.Add(itemProduceReceiptRecord);
            }));
            Parsers.Add(new ImportParser<WorkbenchReceiptRecord>("workbenchreceipts", delegate (WorkbenchReceiptRecord itemWorkbenchReceiptRecord)
            {
                MGSC.Data.WorkbenchReceipts.Add(itemWorkbenchReceiptRecord);
                itemWorkbenchReceiptRecord.GenerateId();
            }));
            Parsers.Add(new NullableRecordParser<DatadiskRecordTemplate>("datadisks", delegate (DatadiskRecordTemplate datadiskRecord)
            {
                CompositeItemRecord itemRecord = (CompositeItemRecord)MGSC.Data.Items.GetRecord(datadiskRecord.Id);
                if (itemRecord != null) // Append to existing chip if already exists
                {
                    DatadiskRecord dataChip = itemRecord.GetRecord<DatadiskRecord>();
                    dataChip.UnlockIds.AddRange(datadiskRecord.UnlockIds);
                }
                else // Create new chip if doesn't exist
                {
                    Logger.LogWarning($"Creating new datachips not implemented. Chip id {datadiskRecord.Id}");
                    // TODO -- new chips have a descriptor attached, not sure how to do this yet
                    //MGSC.Data.Items.AddRecord(datadiskRecord.Id, datadiskRecord);
                    //datadiskRecord.ContentDescriptor = descs.GetDescriptor(datadiskRecord.Id);
                }
            }));
            Parsers.Add(new NullableRecordParser<FactionTemplate>("factionconfig", GameItemCreator.AddItemsToFactions));
            Parsers.Add(new NullableRecordParser<BackpackRecord>("backpacks", delegate (BackpackRecord backpackItem)
            {
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(backpackItem.Id);
                ItemContentDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ItemContentDescriptor>();
                /// Here we determine which values are needed for the backpack.
                /// It won't magically just become an item, sadly.
                backpackItem.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(backpackItem);
            }));
            //Parsers.Add(new NullableRecordParser<ConsumableRecord>("medkits", delegate (MedkitRecord item)
            //{
            //    CustomItemContentDescriptor customItemDescriptor = GetDescriptor(item.Id);
            //    ItemContentDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ConsumableDescriptor>();
            //    item.ContentDescriptor = itemContentDescriptor;
            //    AddItemToGame(item);
            //}));
            Parsers.Add(new NullableRecordParser<ConsumableRecord>("consumables", delegate (ConsumableRecord item)
            {
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(item.Id);
                ConsumableDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ConsumableDescriptor>();
                itemContentDescriptor._useSound = ExtractCustomParameter<AudioClip>(customItemDescriptor.customParameters, "UseEatSound");
                item.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(item);
            }));
            Parsers.Add(new NullableRecordParser<VestTemplate>("vests", delegate (VestTemplate vestItem)
            {
                var gameVest = vestItem.GetOriginal();
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(gameVest.Id);
                VestDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<VestDescriptor>();
                gameVest.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameVest);
            }));
            Parsers.Add(new NullableRecordParser<HelmetTemplate>("helmets", delegate (HelmetTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(gameArmor.Id);
                var itemContentDescriptor = customItemDescriptor.GetOriginal<HelmetDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor);
            }));
            Parsers.Add(new NullableRecordParser<ArmorTemplate>("armors", delegate (ArmorTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(gameArmor.Id);
                ArmorDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ArmorDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor);
            }));
            Parsers.Add(new NullableRecordParser<LeggingsTemplate>("leggings", delegate (LeggingsTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(gameArmor.Id);
                var itemContentDescriptor = customItemDescriptor.GetOriginal<LeggingsDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor);
            }));
            Parsers.Add(new NullableRecordParser<BootsTemplate>("boots", delegate (BootsTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(gameArmor.Id);
                BootsDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<BootsDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor);
            }));
            Parsers.Add(new NullableRecordParser<RepairRecord>("repairs", delegate (RepairRecord item)
            {
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(item.Id);
                RepairDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<RepairDescriptor>();
                customItemDescriptor.customParameters.TryGetValue("customUseSoundPath", out string[] audioPaths);
                if (audioPaths != null && audioPaths.Length > 0)
                {
                    var audioClip = Importer.ImportAudio(audioPaths[0]);
                    if (audioClip != null)
                        itemContentDescriptor.CustomUseSound = audioClip;
                }
                item.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(item);
            }));
            //Parsers.Add(new NullableRecordParser<GrenadeTemplate>("grenades", delegate (GrenadeTemplate item)
            //{
            //    CustomItemContentDescriptor customItemDescriptor = GetDescriptor(item.Id);
            //    GrenadeItemDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<GrenadeItemDescriptor>();

            //    Data.Descriptors.TryGetValue("grenades", out DescriptorsCollection ammoDescriptors);
            //    GrenadeItemDescriptor baseGrenade = ammoDescriptors.GetDescriptor(customItemDescriptor.baseItemId) as GrenadeItemDescriptor;

            //    if (baseGrenade == null) baseGrenade = ammoDescriptors.GetDescriptor("frag_grenade") as GrenadeItemDescriptor;
            //    if (baseGrenade != null)
            //    {
            //        itemContentDescriptor.entityFlySprites = baseGrenade.entityFlySprites;
            //        itemContentDescriptor.entityShadowSprites = baseGrenade.entityShadowSprites;
            //        itemContentDescriptor.ricochetSound = baseGrenade.ricochetSound;
            //        itemContentDescriptor.throwSound = baseGrenade.throwSound;
            //        itemContentDescriptor.fallSound = baseGrenade.fallSound;
            //    }
            //    else
            //    {
            //        Logger.LogWarning($"Base grenade {customItemDescriptor.baseItemId} and \"frag grenade\" do not exist." +
            //            $"\nCustom grenades will not work properly!");
            //        return;
            //    }

            //    itemContentDescriptor.ClearGibsRadiusInPixels = ExtractCustomParameter<int>(customItemDescriptor.customParameters, "ClearGibsRadiusInPixels");
            //    itemContentDescriptor.ShakeCameraOnExplosion = ExtractCustomParameter<bool>(customItemDescriptor.customParameters, "ShakeCameraOnExplosion");
            //    itemContentDescriptor.visualExplsoionDelay = ExtractCustomParameter<float>(customItemDescriptor.customParameters, "visualExplsoionDelay");
            //    itemContentDescriptor.visualReachCellDuration = ExtractCustomParameter<float>(customItemDescriptor.customParameters, "visualReachCellDuration");

            //    item.ContentDescriptor = itemContentDescriptor;
            //    AddItemToGame(item);
            //}));
            Parsers.Add(new NullableRecordParser<TrashRecord>("trash", delegate (TrashRecord item)
            {
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(item.Id);
                ItemContentDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ItemContentDescriptor>();
                item.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(item);
            }));
            Parsers.Add(new NullableRecordParser<FireModeRecordTemplate>("firemodes", delegate (FireModeRecordTemplate item)
            {
                FireModeRecord fireModeRecord = item.GetOriginal();
                FireModeDescriptor fireModeContentDescriptor = ScriptableObject.CreateInstance<FireModeDescriptor>();
                //Logger.LogInfo($"Adding firemode with ID: {item.id} and image at {item.FireModeSpritePath}");
                fireModeContentDescriptor.Icon = Importer.LoadNewSprite(item.FireModeSpritePath);
                fireModeRecord.ContentDescriptor = fireModeContentDescriptor;
                AddFireModeToGame(fireModeRecord);
            }));
        }

        internal static bool LoadLocalization(ConfigTemplate userConfig)
        {
            Dictionary<string, string> localPaths = userConfig.localizationPaths;
            if (localPaths != null || localPaths.Count > 0)
            {
                foreach (KeyValuePair<string, string> filePath in localPaths)
                {
                    try
                    {
                        string folderPath = Path.Combine(rootFolder, filePath.Value);
                        if (!Directory.Exists(folderPath))
                        {
                            Logger.LogWarning($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.");
                            return false;
                        }
                        DirectoryInfo weaponsDirInfo = new DirectoryInfo(folderPath);
                        FileInfo[] files = weaponsDirInfo.GetFiles("*.json");
                        foreach (FileInfo singleFile in files)
                        {
                            string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                            LocalizationTemplate json = JsonConvert.DeserializeObject<LocalizationTemplate>(configItemContent);

                            // for now with the mod being item focused, the template is only concerned with name and shortdesc
                            // this can be expanded later
                            string key = filePath.Key.ToLower();
                            GameItemCreator.AddLocalization(key, "name", json.name);
                            GameItemCreator.AddLocalization(key, "shortdesc", json.shortdesc);

                            //Logger.LogInfo($"Localization loaded successfully for {filePath.Value}");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed for {filePath.Value}.\n{e.Message}\n{e.StackTrace}");
                        continue;
                    }
                }
                return true;
            }
            else
            {
                //Logger.LogInfo($"No Localization file path set.");
                return false;
            }
        }

        internal static bool ImportConfig(string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                Logger.LogError($"Null config path at ImportConfig entry point.");
                return false;
            }
            return ImportConfig(Importer.GetGlobalConfig(configPath), configPath);
        }

        // Create the global config in the assembly folder.
        // You only send the config over, then everything else is automatic.
        internal static bool ImportConfig(ConfigTemplate userConfig, string rootPath)
        {
            rootFolder = rootPath;
            // We should check the root first.
            // See if atleast has the config file...
            if (string.IsNullOrEmpty(rootFolder))
            {
                Logger.LogError($"Root Folder in global config file is empty.");
                return false;
            }

            if (!Directory.Exists(rootFolder))
            {
                Logger.LogError($"Root Folder \"{rootFolder}\" does not exist.");
                return false;
            }

            Logger.LogInfo($"Starting import config from: {rootPath}");
            Importer.ImagePixelScaling = Mathf.Max(1f, userConfig.imagePixelScale);
            // This must include the Import.
            LoadDescriptors(userConfig);
            LoadDefaultParsers();
            try
            {
                LoadLocalization(userConfig);
                foreach (var path in userConfig.folderPaths)
                {
                    if (!ParseFile(path)) continue;
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"Configuration failed for {rootPath}.\n{e.Message}\n{e.StackTrace}");
                return false;
            }
            //finally
            //{
            //    Logger.FlushAdditive();
            //}
        }


        private static bool ParseFile(KeyValuePair<string, string> relativeFolderPath)
        {
            string folderPath = Path.Combine(rootFolder, relativeFolderPath.Value);
            if (!Directory.Exists(folderPath))
            {
                Logger.LogWarning($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.");
                return false;
            }
            var foundParser = Parsers.Find(x => x.Identifier.ToLower().Equals(relativeFolderPath.Key.ToLower()));
            if (foundParser == null)
            {
                Logger.LogWarning($"No parser exists for [{relativeFolderPath.Key}]");
                return false;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");
            foreach (FileInfo singleFile in files)
            {
                Logger.SetContext(singleFile.Name);
                string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                foundParser.Parse(configItemContent);
                Logger.LogInfo($"Finished parsing {singleFile.Name} in {relativeFolderPath}");
                Logger.ClearContext();
            }
            return true;
        }

        #region Utils

        private static void AddItemToGame(BasePickupItemRecord item)
        {
            if (item.ContentDescriptor != null)
            {
                try
                {
                    if (MGSC.Data.Items.GetSimpleRecord<BasePickupItemRecord>(item.Id) != null)
                    {
                        // If a weapon with that ID, by any case, is already registered. OVERRIDE IT.
                        // In the end this will ensure correctness of mods over other ones.
                        // Also creators must ensure IDs are unique.
                        MGSC.Data.Items.RemoveRecord(item.Id);
                        Logger.LogWarning($"An item with ID: [{item.Id}] was OVERRIDEN!!!");
                    }
                }
                catch (Exception ex) { }
                MGSC.Data.Items.AddRecord(item.Id, item);
            }
            else
            {
                Logger.LogError($"Item {item.Id} could not be loaded because descriptor is null.");
            }
        }

        private static void AddItemTransform(ItemTransformationRecord item)
        {
            try
            {
                if (!string.IsNullOrEmpty(MGSC.Data.ItemTransformation.GetRecord(item.Id).Id))
                {
                    // If a weapon with that ID, by any case, is already registered. OVERRIDE IT.
                    // In the end this will ensure correctness of mods over other ones.
                    // Also creators must ensure IDs are unique.
                    MGSC.Data.ItemTransformation.RemoveRecord(item.Id);
                    Logger.LogWarning($"An ItemTransformation with ID: [{item.Id}] was OVERRIDEN!!!");
                }
            }
            catch (Exception ex) { }
            MGSC.Data.ItemTransformation.AddRecord(item.Id, item);
        }

        private static void AddProduceReceipt(ItemProduceReceipt item)
        {
            try
            {
                var existingReceipt = MGSC.Data.ProduceReceipts.Find(x => x.Equals(item.Id));
                if (existingReceipt != null)
                {
                    // If a weapon with that ID, by any case, is already registered. OVERRIDE IT.
                    // In the end this will ensure correctness of mods over other ones.
                    // Also creators must ensure IDs are unique.
                    MGSC.Data.ProduceReceipts.Remove(existingReceipt);
                    Logger.LogWarning($"A ProduceReceipt with ID: [{item.Id}] was OVERRIDEN!!!");
                }
            }
            catch (Exception ex) { }
            MGSC.Data.ProduceReceipts.Add(item);
        }

        private static void AddFireModeToGame(FireModeRecord fireModeRecord)
        {
            if (fireModeRecord.ContentDescriptor != null)
            {
                MGSC.Data.Firemodes.AddRecord(fireModeRecord.Id, fireModeRecord);
            }
            else
            {
                Logger.LogError($"Item {fireModeRecord.Id} could not be loaded because descriptor is null.");
            }
        }

        private static T ExtractCustomParameter<T>(Dictionary<string, string[]> descriptorParameters, string parameter)
        {
            descriptorParameters.TryGetValue(parameter, out string[] customParameters);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.IsValid(customParameters[0]))
                return (T)converter.ConvertFromString(customParameters[0]);
            else return default(T);
        }

        public static CustomItemContentDescriptor GetDescriptor(string id)
        {
            var item = itemDescriptors.Find(x => x.attachedId.Equals(id));
            if (item == null)
            {
                Logger.LogError($"Descriptor for {id} not found. Returning a null");
                return null;
            }
            return item;
        }

        /// <summary>
        /// Modder can use this function to add custom data parsers.
        /// </summary>
        /// <param name="userTemplate"></param>
        public static void AddParser(IConfigParser userTemplate)
        {
            Parsers.Add(userTemplate);
        }

        #endregion

    }
}