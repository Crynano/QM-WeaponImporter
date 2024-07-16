using MGSC;
using QM_WeaponImporter.Templates.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;

namespace QM_WeaponImporter
{
    // The idea of the ConfigManager is that imports the GLOBAL config first. Then each of the paths points to the folder to the configurable items
    // Also, calls to the function should pass the class that specifies every path. So someone using it as an API can send an instance of the class
    // and the config manager uses that to load paths. Otherwise if no file has been sent just load by default.
    // BUt if multiple mods are loading stuff at the same time, they should have the file or class and say, load these weapons please.
    // The class should include the root folder. Maybe a simple ../ would work but let's check.
    // We can assume it wont be needed but yeah.
    public static class ConfigManager
    {
        public static string rootFolder;

        private static List<IConfigParser> Parsers = new List<IConfigParser>();

        // How about we load descriptors first?
        private static List<CustomItemContentDescriptor> itemDescriptors = new List<CustomItemContentDescriptor>();

        private static void LoadDescriptors(ConfigTemplate userConfig)
        {
            Logger.WriteToLog($"Loading descriptors");
            Parsers.Add(new TemplateParser<CustomItemContentDescriptor>("descriptorsPath", itemDescriptors.Add));
            KeyValuePair<string, string> descriptorsEntry = new KeyValuePair<string, string>("descriptorsPath", userConfig.descriptorsPath);
            if (!ParseFile(descriptorsEntry))
            {
                // If it doesn't work, interrupt
                Logger.WriteToLog($"Interrupting Mod Load: Descriptors Folder Path not found in {descriptorsEntry.Value}.\nPlease add them in the {Importer.GlobalConfigName} file.", Logger.LogType.Error);
                Logger.Flush();
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
                Logger.WriteToLog($"Parsing [{ammoItem.Id}]");
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(ammoItem.Id);
                Data.Descriptors.TryGetValue("ammo", out DescriptorsCollection ammoDescriptors);
                AmmoDescriptor baseAmmo = ammoDescriptors.GetDescriptor(customItemDescriptor.baseItemId) as AmmoDescriptor;
                ItemContentDescriptor originalDescriptor = customItemDescriptor.GetOriginal();
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
                MGSC.Data.Items.AddRecord(ammoItem.Id, ammoItem);
            }));
            Parsers.Add(new TemplateParser<MeleeWeaponTemplate>("meleeweapons", delegate (MeleeWeaponTemplate weaponTemplate)
            {
                GameItemCreator.CreateMeleeWeapon(weaponTemplate);
            }));
            // TODO -- port this to the ImportParser
            // ------- eliminate RangedWeaponTemplate
            Parsers.Add(new TemplateParser<RangedWeaponTemplate>("rangedweapons", delegate (RangedWeaponTemplate weaponTemplate)
            {
                GameItemCreator.CreateRangedWeapon(weaponTemplate);
            }));
            Parsers.Add(new ImportParser<ItemTransformationRecord>("itemtransforms", delegate (ItemTransformationRecord itemTransformRecord)
            {
                MGSC.Data.ItemTransformation.AddRecord(itemTransformRecord.Id, itemTransformRecord);
            }));
            Parsers.Add(new ImportParser<ItemProduceReceipt>("itemreceipts", delegate (ItemProduceReceipt itemProduceReceiptRecord)
            {
                MGSC.Data.ProduceReceipts.Add(itemProduceReceiptRecord);
            }));
            Parsers.Add(new ImportParser<WorkbenchReceiptRecord>("workbenchreceipts", delegate (WorkbenchReceiptRecord itemWorkbenchReceiptRecord)
            {
                MGSC.Data.WorkbenchReceipts.Add(itemWorkbenchReceiptRecord);
                itemWorkbenchReceiptRecord.GenerateId();
            }));
            Parsers.Add(new ImportParser<DatadiskRecord>("datadisks", delegate (DatadiskRecord datadiskRecord)
            {
                CompositeItemRecord itemRecord = (CompositeItemRecord)MGSC.Data.Items.GetRecord(datadiskRecord.Id);
                if (itemRecord != null) // Append to existing chip if already exists
                {
                    DatadiskRecord dataChip = itemRecord.GetRecord<DatadiskRecord>();
                    dataChip.UnlockIds.AddRange(datadiskRecord.UnlockIds);
                }
                else // Create new chip if doesn't exist
                {
                    Logger.WriteToLog($"Creating new datachips not implemented. Chip id {datadiskRecord.Id}");
                    // TODO -- new chips have a descriptor attached, not sure how to do this yet
                    //MGSC.Data.Items.AddRecord(datadiskRecord.Id, datadiskRecord);
                    //datadiskRecord.ContentDescriptor = descs.GetDescriptor(datadiskRecord.Id);
                }
            }));
            // TODO -- port this to the ImportParser
            // ------- eliminate FactionTemplate
            Parsers.Add(new TemplateParser<FactionTemplate>("factionitems", delegate (FactionTemplate factionTemplate)
            {
                GameItemCreator.AddItemsToFactions(factionTemplate);
            }));
            // TODO -- port this to the ImportParser
            // ------- reference datadiskRecord parses for completing backpack implementation
            Parsers.Add(new NullableRecordParser<BackpackRecord>("backpacks", delegate (BackpackRecord backpackItem)
            {
                Logger.WriteToLog($"Backpack ID: [{backpackItem.Id}]");
                CustomItemContentDescriptor customItemDescriptor = GetDescriptor(backpackItem.Id);
                ItemContentDescriptor itemContentDescriptor = customItemDescriptor.GetOriginal();
                /// Here we determine which values are needed for the backpack.
                /// It won't magically just become an item, sadly.
                backpackItem.ContentDescriptor = itemContentDescriptor;
                if (backpackItem.ContentDescriptor != null)
                {
                    MGSC.Data.Items.AddRecord(backpackItem.Id, backpackItem);
                }
                else
                {
                    Logger.WriteToLog($"Backpack {backpackItem.Id} could not be loaded because descriptor is null.", Logger.LogType.Warning);
                }
            }));

        }

        // Create the global config in the assembly folder.
        // You only send the config over, then everything else is automatic.
        public static bool ImportConfig(ConfigTemplate userConfig)
        {
            Logger.WriteToLog($"Starting import config from: {userConfig.rootFolder}");
            rootFolder = userConfig.rootFolder;
            // This must include the Import.
            LoadDescriptors(userConfig);
            LoadDefaultParsers();
            try
            {

                if (rootFolder == null || rootFolder.Equals(string.Empty))
                {
                    Logger.WriteToLog($"Root Folder in global config file is empty.", Logger.LogType.Error);
                    return false;
                }
                if (!Directory.Exists(rootFolder))
                {
                    Logger.WriteToLog($"Root Folder \"{rootFolder}\" does not exist.", Logger.LogType.Error);
                    return false;
                }

                Logger.WriteToLog($"Iterating through folders");
                foreach (var path in userConfig.folderPaths)
                {
                    if (!ParseFile(path)) continue;
                }
                Logger.WriteToLog($"Configuration success for {userConfig.rootFolder}");
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Configuration failed for {userConfig.rootFolder}.\n{e.Message}\n{e.StackTrace}", Logger.LogType.Error);
                return false;
            }
        }

        private static bool ParseFile(KeyValuePair<string, string> relativeFolderPath)
        {
            string folderPath = Path.Combine(rootFolder, relativeFolderPath.Value);
            if (!Directory.Exists(folderPath))
            {
                Logger.WriteToLog($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.", Logger.LogType.Warning);
                return false;
            }
            Logger.WriteToLog($"Searching for {folderPath}");
            var foundParser = Parsers.Find(x => x.Identifier.ToLower().Equals(relativeFolderPath.Key.ToLower()));
            if (foundParser == null)
            {
                Logger.WriteToLog($"No parser exists for [{relativeFolderPath.Key}]", Logger.LogType.Warning);
                return false;
            }
            Logger.WriteToLog($"Checking for {relativeFolderPath}");
            DirectoryInfo weaponsDirInfo = new DirectoryInfo(folderPath);
            FileInfo[] files = weaponsDirInfo.GetFiles("*.json");
            foreach (FileInfo singleFile in files)
            {
                Logger.WriteToLog($"Iterating through {singleFile.Name}");
                string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                foundParser.Parse(configItemContent);
                Logger.WriteToLog($"Finished parsing {singleFile.Name} in {relativeFolderPath}");
            }
            return true;
        }

        #region Utils
        public static CustomItemContentDescriptor GetDescriptor(string id)
        {
            return itemDescriptors.Find(x => x.attachedId.Equals(id));
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