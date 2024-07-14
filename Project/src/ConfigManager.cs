using MGSC;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
using QM_WeaponImporter.Templates.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Mesh;

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
        private static List<CustomItemContentDescriptor> itemDescriptors = new List<CustomItemContentDescriptor>()
        {
            new CustomBackpackDescriptor()
            {
                overridenRenderId = "medicbackpack",
                iconSpritePath = "Assets/Images/medicbackpack.png",
                smallIconSpritePath = "Assets/Images/medicbackpack.png",
                shadowOnFloorSpritePath = "Assets/Images/medicbackpack.png"
            }
        };

        public static void LoadDefaultParsers()
        {
            // We can just copy the code they used xD.
            // TODO -- port this to the ImportParser
            // ------- eliminate MeleeWeaponTemplate
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
                //GameItemCreator.CreateRangedWeapon(factionTemplate);
                GameItemCreator.AddItemsToFactions(factionTemplate);
            }));
            // TODO -- port this to the ImportParser
            // ------- reference datadiskRecord parses for completing backpack implementation
            Parsers.Add(new NullableRecordParser<BackpackRecord>("backpacks", delegate (BackpackRecord backpackItem)
            {
                Logger.WriteToLog($"The ID for Backpack from import is {backpackItem.Id}");
                backpackItem.ContentDescriptor = GetDescriptor<BackpackDescriptor>(backpackItem.Id);
                MGSC.Data.Items.AddRecord(backpackItem.Id, backpackItem);
            }));
        }

        public static T GetDescriptor<T>(string id) where T : ItemContentDescriptor
        {
            // Transform it first no?
            Logger.WriteToLog($"Get descriptor for {id}");
            return itemDescriptors.Find(x => x.attachedId.Equals(id)).GetOriginal() as T;
        }

        /// <summary>
        /// Modder can use this function to add custom data parsers.
        /// </summary>
        /// <param name="userTemplate"></param>
        public static void AddParser(IConfigParser userTemplate)
        {
            Parsers.Add(userTemplate);
        }

        // Create the global config in the assembly folder.
        // You only send the config over, then everything else is automatic.
        public static bool ImportConfig(ConfigTemplate userConfig)
        {
            try
            {
                Logger.WriteToLog($"Starting import config from: {userConfig.rootFolder}");
                rootFolder = userConfig.rootFolder;
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
                    string folderPath = Path.Combine(rootFolder, path.Value);
                    if (!Directory.Exists(folderPath))
                    {
                        Logger.WriteToLog($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.", Logger.LogType.Warning);
                        continue;
                    }
                    var foundParser = Parsers.Find(x => x.Identifier.Equals(path.Key.ToLower()));
                    if (foundParser == null)
                    {
                        Logger.WriteToLog($"No parser exists for [{path.Key}]", Logger.LogType.Warning);
                        continue;
                    }
                    Logger.WriteToLog($"Checking for {path}");
                    DirectoryInfo weaponsDirInfo = new DirectoryInfo(folderPath);
                    FileInfo[] files = weaponsDirInfo.GetFiles("*.json");
                    foreach (FileInfo singleFile in files)
                    {
                        Logger.WriteToLog($"Iterating through {singleFile.Name}");
                        string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                        foundParser.Parse(configItemContent);
                    }
                }
                Logger.WriteToLog($"Configuration success for {userConfig.rootFolder}");
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Configuration failed for {userConfig.rootFolder}.\n{e.Message}\n{e.InnerException}", Logger.LogType.Error);
                return false;
            }
        }

        //private static WeaponTemplate TypeToClass(WeaponTemplate type, string content)
        //{
        //    switch (type)
        //    {
        //        case MeleeWeaponTemplate:
        //            // DAMN THIS IS ALLOWED!
        //            return DynamicDeserializer<MeleeWeaponTemplate>(content);
        //        case RangedWeaponTemplate:
        //            return DynamicDeserializer<RangedWeaponTemplate>(content);
        //        default:
        //            return null;
        //    }
        //}

        private static WeaponTemplate TypeToClass(string type, string content)
        {
            switch (type)
            {
                case "meleeweapons":
                    return DynamicDeserializer<MeleeWeaponTemplate>(content);
                case "rangedweapons":
                    return DynamicDeserializer<RangedWeaponTemplate>(content);
                default:
                    Logger.WriteToLog($"No serializer exists for this weapon {type}");
                    return null;
            }
        }

        private static T DynamicDeserializer<T>(string text)
        {
            var result = JsonConvert.DeserializeObject<T>(text);
            Logger.WriteToLog($"{result}");
            return result;
        }
    }
}