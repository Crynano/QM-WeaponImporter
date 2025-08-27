using MGSC;
using Newtonsoft.Json;
using QM_WeaponImporter.Services;
using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using QM_WeaponImporter.ErrorManagement;
using QM_WeaponImporter.Interfaces;
using UnityEngine;

namespace QM_WeaponImporter
{
    // The idea of the ConfigManager is that imports the GLOBAL config first. Then each of the paths points to the folder to the configurable items
    // Also, calls to the function should pass the class that specifies every path. So someone using it as an API can send an instance of the class
    // and the config manager uses that to load paths. Otherwise if no file has been sent just load by default.
    // BUt if multiple mods are loading stuff at the same time, they should have the file or class and say, load these weapons please.
    // The class should include the root folder. Maybe a simple ../ would work but let's check.
    // We can assume it wont be needed but yeah.
    internal class DataParser
    {
        public static string RootFolder;

        private static List<IConfigParser> _parsers = new List<IConfigParser>();

        // How about we load descriptors first?
        // This needs to be cleaned after every mod?
        private List<CustomBaseDescriptor> _itemDescriptors = new List<CustomBaseDescriptor>();

        private ItemCreatorHelper itemCreator;
        
        private Action<string> OnItemAddedToData;

        public DataParser()
        {
            // Initialize parsers for everything first.
            // Then, if already initialized, just skip this part!
            if (_parsers.Count > 0) return;

            itemCreator = new ItemCreatorHelper();
            InitializeDescriptorParsers();
            InitializeItemParsers();
        }

        private void InitializeDescriptorParsers()
        {
            // Old implementation, keep for retro-compatibility
            _parsers.Add(new TemplateParser<CustomWeaponDescriptor>("descriptorsPath", _itemDescriptors.Add));
            
            // Now that we have a list, we can go through it.
            // Add all parsers for custom content descriptors.
            // Weapons
            _parsers.Add(new TemplateParser<CustomWeaponDescriptor>("weaponsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomAmmoDescriptor>("ammoDescriptors", _itemDescriptors.Add));
            
            // Consumables
            _parsers.Add(new TemplateParser<CustomConsumableDescriptor>("consumableDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("trashDescriptors", _itemDescriptors.Add));
            
            // Datadisks
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("datadiskDescriptors", _itemDescriptors.Add));
            
            // Armor Sets
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("helmetsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("armorsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("leggingsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("bootsDescriptors", _itemDescriptors.Add));
            
            // Boom
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("grenadesDescriptors", _itemDescriptors.Add));
            
            // Augments and WoundSlots
            _parsers.Add(new TemplateParser<CustomItemContentDescriptor>("augmentationsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomWoundSlotDescriptor>("woundSlotsDescriptors", _itemDescriptors.Add));
            _parsers.Add(new TemplateParser<CustomImplantDescriptor>("implantsDescriptors", _itemDescriptors.Add));
            
            // Firemodes
            _parsers.Add(new TemplateParser<CustomFireModeDescriptor>("firemodesDescriptors", delegate (CustomFireModeDescriptor fireModeDesc)
            {
                Logger.LogDebug($"Adding custom firemode descriptor with ID: \"{fireModeDesc.attachedId}\"");
                _itemDescriptors.Add(fireModeDesc);
            }));
        }

        private void InitializeItemParsers()
        {
            _parsers.Add(new NullableRecordParser<ItemTraitRecordTemplate>("trait", delegate (ItemTraitRecordTemplate itemTraitRecord)
            {
                // Works outta the box?
                ItemTraitRecord realRecord = itemTraitRecord.GetOriginal();
                AddTraitToGame(realRecord);
            }));
            _parsers.Add(new NullableRecordParser<WoundSlotRecord>("woundSlots", delegate (WoundSlotRecord item)
            {
                WoundSlotDescriptor itemContentDescriptor = GetDescriptor<CustomWoundSlotDescriptor>(item.Id).GetOriginal();
                item.ContentDescriptor = itemContentDescriptor;
                MGSC.Data.WoundSlots.AddRecord(item.Id, item);
            }));
            _parsers.Add(new NullableRecordParser<AugmentationRecordTemplate>("augmentations", delegate (AugmentationRecordTemplate item)
            {
                ItemContentDescriptor itemContentDescriptor = GetDescriptor<CustomItemContentDescriptor>(item.Id).GetOriginal<ItemContentDescriptor>();
                var original = item.GetOriginal();
                original.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(original, "augmentations");
            }));
            _parsers.Add(new NullableRecordParser<ImplantRecord>("implants", delegate (ImplantRecord item)
            {
                ImplantDescriptor itemContentDescriptor = GetDescriptor<CustomImplantDescriptor>(item.Id).GetOriginal();
                item.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(item, "implants");
            }));
            _parsers.Add(new NullableRecordParser<AmmoRecordTemplate>("ammo", delegate (AmmoRecordTemplate ammoItem)
            {
                AmmoRecord ammoRecord = ammoItem.GetOriginal();
                AmmoDescriptor ammoDesc = GetDescriptor<CustomAmmoDescriptor>(ammoItem.Id).GetOriginal<AmmoDescriptor>();
                ammoRecord.ContentDescriptor = ammoDesc;
                AddItemToGame(ammoRecord, "ammo");
            }));
            _parsers.Add(new TemplateParser<MeleeWeaponTemplate>("meleeweapons", delegate (MeleeWeaponTemplate weaponTemplate)
            {
                Logger.SetContext(weaponTemplate.Id);
                var added = itemCreator.CreateWeapon(weaponTemplate, GetDescriptor<CustomWeaponDescriptor>(weaponTemplate.Id));
                if (added) OnItemAddedToData?.Invoke(weaponTemplate.Id);
            }));
            // TODO -- port this to the ImportParser
            // ------- eliminate RangedWeaponTemplate
            _parsers.Add(new TemplateParser<RangedWeaponTemplate>("rangedweapons", delegate (RangedWeaponTemplate weaponTemplate)
            {
                Logger.SetContext(weaponTemplate.Id);
                var added = itemCreator.CreateWeapon(weaponTemplate, GetDescriptor<CustomWeaponDescriptor>(weaponTemplate.Id));
                if (added) OnItemAddedToData?.Invoke(weaponTemplate.Id);
            }));
            _parsers.Add(new ImportParser<ItemTransformationRecord>("itemtransforms", AddItemTransform));
            _parsers.Add(new ImportParser<ItemProduceReceipt>("itemreceipts", AddProduceReceipt));
            _parsers.Add(new ImportParser<WorkbenchReceiptRecord>("workbenchreceipts", delegate (WorkbenchReceiptRecord itemWorkbenchReceiptRecord)
            {
                MGSC.Data.WorkbenchReceipts.Add(itemWorkbenchReceiptRecord);
                itemWorkbenchReceiptRecord.GenerateId();
            }));
            _parsers.Add(new NullableRecordParser<DatadiskRecordTemplate>("datadisks", delegate (DatadiskRecordTemplate datadiskRecord)
            {
                CompositeItemRecord itemRecord = (CompositeItemRecord)MGSC.Data.Items.GetRecord(datadiskRecord.Id);
                if (itemRecord != null) // Append to existing chip if already exists
                {
                    DatadiskRecord dataChip = itemRecord.GetRecord<DatadiskRecord>();
                    dataChip.UnlockIds.AddRange(datadiskRecord.UnlockIds);
                }
                else // Create new chip if doesn't exist
                {
                    ItemContentDescriptor descriptor = GetDescriptor<CustomItemContentDescriptor>(datadiskRecord.Id).GetOriginal<ItemContentDescriptor>();
                    DatadiskRecord diskRecord = datadiskRecord.GetOriginal();
                    diskRecord.ContentDescriptor = descriptor;
                    AddItemToGame(diskRecord, "datadisks");
                }
            }));
            _parsers.Add(new NullableRecordParser<FactionTemplate>("factionconfig", itemCreator.AddItemsToFactions));
            _parsers.Add(new NullableRecordParser<BackpackRecord>("backpacks", delegate (BackpackRecord backpackItem)
            {
                ItemContentDescriptor itemContentDescriptor = GetDescriptor<CustomItemContentDescriptor>(backpackItem.Id).GetOriginal<ItemContentDescriptor>();
                backpackItem.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(backpackItem, "backpacks");
            }));
            _parsers.Add(new NullableRecordParser<ConsumableRecord>("consumables", delegate (ConsumableRecord item)
            {
                // TODO CHECK IF NEW METHOD WORKS
                //var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(item.Id);
                //ConsumableDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ConsumableDescriptor>();
                //itemContentDescriptor._useSound = Importer.ImportAudio(ExtractCustomParameter(customItemDescriptor.customParameters, "UseEatSound"));
                //item.ContentDescriptor = itemContentDescriptor;
                // THIS BELOW IS THE NEW METHOD
                var consumableDescriptor = GetDescriptor<CustomConsumableDescriptor>(item.Id).GetOriginal<ConsumableDescriptor>();
                item.ContentDescriptor = consumableDescriptor;
                AddItemToGame(item, "consumables");
            }));
            _parsers.Add(new NullableRecordParser<VestTemplate>("vests", delegate (VestTemplate vestItem)
            {
                var gameVest = vestItem.GetOriginal();
                var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(gameVest.Id);
                VestDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<VestDescriptor>();
                gameVest.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameVest, "vests");
            }));

            _parsers.Add(new NullableRecordParser<HelmetTemplate>("helmets", delegate (HelmetTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(gameArmor.Id);
                var itemContentDescriptor = customItemDescriptor.GetOriginal<HelmetDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor, "helmets");
            }));
            _parsers.Add(new NullableRecordParser<ArmorTemplate>("armors", delegate (ArmorTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(gameArmor.Id);
                ArmorDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<ArmorDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor, "armors");
            }));
            _parsers.Add(new NullableRecordParser<LeggingsTemplate>("leggings", delegate (LeggingsTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(gameArmor.Id);
                var itemContentDescriptor = customItemDescriptor.GetOriginal<LeggingsDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor, "leggings");
            }));
            _parsers.Add(new NullableRecordParser<BootsTemplate>("boots", delegate (BootsTemplate armorItem)
            {
                var gameArmor = armorItem.GetOriginal();
                var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(gameArmor.Id);
                BootsDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<BootsDescriptor>();
                gameArmor.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(gameArmor, "boots");
            }));
            _parsers.Add(new NullableRecordParser<RepairRecord>("repairs", delegate (RepairRecord item)
            {
                throw new NotImplementedException("Repairs are not implemented");
                // var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(item.Id);
                // RepairDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal<RepairDescriptor>();
                // customItemDescriptor.customParameters.TryGetValue("customUseSoundPath", out string[] audioPaths);
                // if (audioPaths != null && audioPaths.Length > 0)
                // {
                //     var audioClip = Importer.ImportAudio(audioPaths[0]);
                //     if (audioClip != null)
                //         itemContentDescriptor.CustomUseSound = audioClip;
                // }
                // item.ContentDescriptor = itemContentDescriptor;
                // AddItemToGame(item);
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
            _parsers.Add(new NullableRecordParser<TrashRecord>("trash", delegate (TrashRecord item)
            {
                ItemContentDescriptor itemContentDescriptor = GetDescriptor<CustomItemContentDescriptor>(item.Id).GetOriginal<ItemContentDescriptor>();
                item.ContentDescriptor = itemContentDescriptor;
                AddItemToGame(item, "trash");
            }));
            _parsers.Add(new NullableRecordParser<FireModeRecordTemplate>("firemodes", delegate (FireModeRecordTemplate item)
            {
                FireModeRecord fireModeRecord = item.GetOriginal();
                FireModeDescriptor fireModeDesc = GetDescriptor<CustomFireModeDescriptor>(item.Id).GetOriginal<FireModeDescriptor>();
                fireModeRecord.ContentDescriptor = fireModeDesc;
                AddFireModeToGame(fireModeRecord, "firemodes");
            }));
        }
        
        private void LoadDescriptors(ConfigTemplate userConfig)
        {
            KeyValuePair<string, string> descriptorsEntry = new KeyValuePair<string, string>("descriptorsPath", userConfig.descriptorsPath);
            if (!ParseFile(descriptorsEntry))
            {
                // If it doesn't work, interrupt
                Logger.LogError($"Interrupting Mod Load: Descriptors Folder Path not found in {descriptorsEntry.Value}.\nPlease add them in the {Importer.GlobalConfigName} file.");
                throw new NullReferenceException($"Critical error: Descriptors Folder Path not found in {descriptorsEntry.Value}.\nPlease add them in the {Importer.GlobalConfigName} file.");
            }
            
            try
            {
                var descDictionary = userConfig.descriptorsPaths;
                var orderedDic = descDictionary.OrderBy(x => GetDescriptorsOrder().FindIndex(y => x.Key.Contains(y.ToLower())));
                Logger.LogDebug($"Printing ordered dictionary!");
                foreach (var singleDescriptorDictionary in orderedDic)
                {
                    Logger.LogDebug(singleDescriptorDictionary.Key);
                    Logger.LogDebug($"Trying to load new method descriptor at {singleDescriptorDictionary}");
                    if (!ParseFile(singleDescriptorDictionary)) continue;
                }
            }
            catch (NullReferenceException ex)
            {
                Logger.LogError($"Descriptors dictionary missing in {Importer.GlobalConfigName} file. Some weapons might not load at all or prevent the mod from loading. Please check the following example files and ensure that \"global_config.json\" file contains the \"descriptorsPaths\" properties.");
                //ExamplesManager.CreateExampleFiles(RootFolder);
            }
        }

        /// <summary>
        /// Callable for any item that goes into Data.Items and is an item.
        /// </summary>
        internal void ProcessItem(IOriginalCopy<ItemRecord> originalItem, string descriptorCategory)
        {
            var itemRecord = originalItem.GetOriginal();
            var customItemDescriptor = GetDescriptor<CustomItemContentDescriptor>(itemRecord.Id).GetOriginal<VestDescriptor>();
            itemRecord.ContentDescriptor = customItemDescriptor;
            AddItemToGame(itemRecord, descriptorCategory);
        }

        internal bool LoadLocalization(ConfigTemplate userConfig)
        {
            Dictionary<string, string> localPaths = userConfig.localizationPaths;
            if (localPaths != null || localPaths.Count > 0)
            {
                foreach (KeyValuePair<string, string> filePath in localPaths)
                {
                    try
                    {
                        string folderPath = Path.Combine(RootFolder, filePath.Value);
                        if (!Directory.Exists(folderPath))
                        {
                            Logger.LogWarning($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.");
                            continue;
                        }
#if DEBUG
                        Logger.LogInfo($"Adding localization from {folderPath} for {filePath.Key}");
#endif
                        DirectoryInfo weaponsDirInfo = new DirectoryInfo(folderPath);
                        FileInfo[] files = weaponsDirInfo.GetFiles("*.json");
                        foreach (FileInfo singleFile in files)
                        {
                            string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                            LocalizationTemplate json;
                            try
                            {
                                json = JsonConvert.DeserializeObject<LocalizationTemplate>(configItemContent);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Could not deserialize localization file. Error:{ex.Message}");
                                continue;
                            }

                            // for now with the mod being item focused, the template is only concerned with name and shortdesc
                            // this can be expanded later
                            // Now that we are expanding, ignore the ToLower requirement.
                            // And also, use the table name instead of a hardcoded entry
                            string key = filePath.Key; //.ToLower();
                            string nameGroup = "name";
                            string descGroup = "shortdesc";
                            switch (key)
                            {
                                case "station":
                                    descGroup = "type";
                                    itemCreator.AddLocalization(key, "shortname", json.name);
                                    break;
                                case "alliance":
                                    descGroup = "subName";
                                    break;
                                case "faction":
                                    descGroup = "desc";
                                    break;
                            }
                            itemCreator.AddLocalization(key, nameGroup, json.name);
                            itemCreator.AddLocalization(key, descGroup, json.shortdesc);

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

        // Create the global config in the assembly folder.
        // You only send the config over, then everything else is automatic.
        internal void ImportConfig(ConfigTemplate userConfig, string rootPath, ref ResultInfo resultInfo)
        {
            DateTime now = DateTime.Now;
            RootFolder = rootPath;
            OnItemAddedToData = resultInfo.AddItem;
            
            // We should check the root first.
            // See if atleast has the config file...
            if (string.IsNullOrEmpty(RootFolder))
            {
                Logger.LogError($"Root Folder is empty.");
                resultInfo.Result = false;
                resultInfo.ResultMessage = "Root folder is empty";
                return;
            }

            if (!Directory.Exists(RootFolder))
            {
                Logger.LogError($"Root Folder \"{RootFolder}\" does not exist.");
                resultInfo.Result = false;
                resultInfo.ResultMessage = $"Folder \"{RootFolder}\" does not exist.";
                return;
            }

            Logger.LogInfo($"Starting import config from: {rootPath}");
            Importer.ImagePixelScaling = Mathf.Max(1f, userConfig.imagePixelScale);
            
            // This must include the Import.
            LoadDescriptors(userConfig);
            try
            {
                LoadLocalization(userConfig);
                var orderedFolders = OrderByType(userConfig.folderPaths);
                foreach (var path in orderedFolders)
                {
                    //Logger.LogDebug($"Trying ordered folders! \"{path.Key}\" and \"{path.Value}\"");
                    if (!ParseFile(path)) continue;
                }
                return;
            }
            catch (Exception e)
            {
                Logger.LogError($"Configuration failed for {rootPath}.\n{e.Message}\n{e.StackTrace}");
                resultInfo.ResultMessage = $"Configuration failed for {rootPath}.\n{e.Message}\n{e.StackTrace}";
                resultInfo.Result = false;
            }
            finally
            {
                // We clean the descriptors so another mod can use it.
                Logger.LogInfo($"Import process took {resultInfo.ExecutionTime:0.00} seconds.");
                resultInfo.ResultMessage = "Successfully imported.";
                resultInfo.ExecutionTime = (DateTime.Now - now).TotalSeconds;
                _itemDescriptors.Clear();
            }
            return;
        }


        private  bool ParseFile(KeyValuePair<string, string> relativeFolderPath)
        {
            string folderPath = Path.Combine(RootFolder, relativeFolderPath.Value);
            if (!Directory.Exists(folderPath))
            {
                Logger.LogWarning($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.");
                return false;
            }
            
            var foundParser = _parsers.Find(x => x.Identifier.ToLower().Equals(relativeFolderPath.Key.ToLower()));
            if (foundParser == null)
            {
                Logger.LogWarning($"No parser exists for \"{relativeFolderPath.Key}\"");
                return false;
            }
            
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");
            foreach (FileInfo singleFile in files)
            {
                Logger.SetContext(singleFile.Name);
                try
                {
                    string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                    foundParser.Parse(configItemContent);
                    Logger.LogInfo($"Successfully parsed: \"{singleFile.Name}\" in \"{relativeFolderPath}\"");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception when parsing file \"{singleFile.Name}\" in \"{relativeFolderPath}\".\n{ex.Message}");
                }
                finally
                {
                    Logger.ClearContext();
                }
            }
            return true;
        }

        #region Utils
        private  void AddTraitToGame(ItemTraitRecord traitRecord)
        {
            MGSC.Data.ItemTraits.AddRecord(traitRecord.Id, traitRecord);
        }

        private  void AddItemToGame(BasePickupItemRecord item, string descriptorCategory)
        {
            if (item.ContentDescriptor != null)
            {
                // try
                // {
                //     if (MGSC.Data.Items.GetSimpleRecord<BasePickupItemRecord>(item.Id) != null)
                //     {
                //         // If a weapon with that ID, by any case, is already registered. OVERRIDE IT.
                //         // In the end this will ensure correctness of mods over other ones.
                //         // Also creators must ensure IDs are unique.
                //         //MGSC.Data.Items.RemoveRecord(item.Id);
                //         Logger.LogWarning($"An item with ID: [{item.Id}] would have been overriden.");
                //     }
                // }
                // catch (Exception ex)
                // {
                //     // ignored
                // }
                if (MGSC.Data.Descriptors.ContainsKey(descriptorCategory))
                {
                    MGSC.Data.Descriptors[descriptorCategory].AddDescriptor(item.Id, item.ItemDesc);
                }
                else
                {
                    Logger.LogWarning($"Descriptor category \"{descriptorCategory}\" does not exist. Descriptor for {item.Id} not added in MGSC.Descriptors");
                }
                OnItemAddedToData?.Invoke(item.Id);
                MGSC.Data.Items.AddRecord(item.Id, item);
            }
            else
            {
                Logger.LogError($"{item.Id} could not be loaded because descriptor is null.");
            }
        }

        private  void AddItemTransform(ItemTransformationRecord item)
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
            try
            {
                if (MGSC.Data.Items.GetSimpleRecord<BasePickupItemRecord>(item.Id) != null)
                {
                    MGSC.Data.ItemTransformation.AddRecord(item.Id, item);
                }
                else
                {
                    Logger.LogError($"Error when checking for Item Transformation with {item.Id}");
                }
            }
            catch (NullReferenceException nullRef)
            {
                // Throw a warning informing that this has skipped.
                Logger.LogWarning($"Item Transformation for \"{item.Id}\" has been skipped. The item has not been found in Data.Items");
            }
        }

        private  void AddProduceReceipt(ItemProduceReceipt item)
        {
            try
            {
                var existingReceipt = MGSC.Data.ProduceReceipts.Find(x => x.Id.Equals(item.Id));
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
            try
            {
                if (MGSC.Data.Items.GetSimpleRecord<BasePickupItemRecord>(item.OutputItem) != null)
                {
                    MGSC.Data.ProduceReceipts.Add(item);
                }
            }
            catch (NullReferenceException nullRef)
            { 
                // Throw a warning informing that this has skipped.
                Logger.LogWarning($"Produce Receipt for \"{item.OutputItem}\" has been skipped. The item has not been found in Data.Items");
            }
        }

        private  void AddFireModeToGame(FireModeRecord fireModeRecord, string descriptorCategory)
        {
            if (fireModeRecord.ContentDescriptor != null)
            {
                if (MGSC.Data.Descriptors.ContainsKey(descriptorCategory))
                {
                    MGSC.Data.Descriptors[descriptorCategory].AddDescriptor(fireModeRecord.Id, fireModeRecord.ContentDescriptor);
                }
                else
                {
                    Logger.LogWarning($"Descriptor category \"{descriptorCategory}\" does not exist. Descriptor for firemode: \"{fireModeRecord.Id}\" not added in MGSC.Descriptors");
                }
                MGSC.Data.Firemodes.AddRecord(fireModeRecord.Id, fireModeRecord);
            }
            else
            {
                Logger.LogError($"Item {fireModeRecord.Id} could not be loaded because descriptor is null.");
            }
        }

        public  T GetDescriptor<T>(string id) where T : CustomBaseDescriptor
        {
            var item = _itemDescriptors.Find(x => x.attachedId.Equals(id));
            if (item == null)
            {
                Logger.LogError($"Descriptor for \"{id}\" not found.");
                return null;
            }
            return item as T;
        }

        private  Dictionary<string, string> OrderByType(Dictionary<string, string> dictionary)
        {
            List<string> firstOrder = new List<string>() { "trait", "ammo", "firemode" };
            List<string> lastOrder = new List<string>() { "transform", "receipts" };
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> end = new Dictionary<string, string>();
            // First Traits
            // Then Ammo
            // Then Firemodes
            // The rest
            // Crafting recipes last
            foreach (string item in firstOrder)
            {
                var indexOfValue = dictionary.Keys.ToList().FindIndex(x => x.Contains(item));
                if (indexOfValue != -1)
                {
                    var res = dictionary.ElementAt(indexOfValue);
                    result.Add(res.Key, res.Value);
                    dictionary.Remove(res.Key);
                }
            }

            var endResult = new Dictionary<string, string>();
            foreach (string item in lastOrder)
            {
                var indexOfValue = dictionary.Keys.ToList().FindIndex(x => x.Contains(item));
                if (indexOfValue != -1)
                {
                    var res = dictionary.ElementAt(indexOfValue);
                    endResult.Add(res.Key, res.Value);
                    dictionary.Remove(res.Key);
                }
            }

            dictionary.ToList().ForEach(x => result.Add(x.Key, x.Value));
            endResult.ToList().ForEach(x => result.Add(x.Key, x.Value));

            return result;
        }

        private  List<string> GetDescriptorsOrder()
        {
            return new List<string>()
            {
                "ammo",
                "firemode",
                "weapons"
            };
        }

        //private  string ExtractCustomParameter(Dictionary<string, string[]> descriptorParameters, string parameter)
        //{
        //    descriptorParameters.TryGetValue(parameter, out string[] customParameters);
        //    if (descriptorParameters == null || customParameters == null) return null;
        //    return customParameters[0];
        //}


        /// <summary>
        /// Modder can use this function to add custom data parsers.
        /// </summary>
        /// <param name="userTemplate"></param>
        public  void AddParser(IConfigParser userTemplate)
        {
            _parsers.Add(userTemplate);
        }

        #endregion

    }
}