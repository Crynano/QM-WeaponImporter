using MGSC;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;
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
    public static class ConfigManager
    {
        public static string rootFolder;

        private static List<IConfigParser> Parsers = new List<IConfigParser>();

        public static void LoadDefaultParsers()
        {
            // We can just copy the code they used xD.
            Parsers.Add(new TemplateParser<MeleeWeaponTemplate>("meleeweapons", delegate (MeleeWeaponTemplate weaponTemplate)
            {
                GameItemCreator.CreateMeleeWeapon(weaponTemplate);
            }));
            Parsers.Add(new TemplateParser<RangedWeaponTemplate>("rangedweapons", delegate (RangedWeaponTemplate weaponTemplate)
            {
                GameItemCreator.CreateRangedWeapon(weaponTemplate);
            }));
            Parsers.Add(new TemplateParser<ItemTransformTemplate>("itemtransforms", delegate (ItemTransformTemplate itemTransformRecord)
            {
                ItemTransformationRecord myTransform = new ItemTransformationRecord();
                myTransform = myTransform.Clone(itemTransformRecord.id);
                myTransform.OutputItems = itemTransformRecord.outputItems;
                Logger.WriteToLog($"Output items for {itemTransformRecord.id} are {myTransform.OutputItems} and was {itemTransformRecord.outputItems}");
                MGSC.Data.ItemTransformation.AddRecord(myTransform.Id, myTransform);
            }));
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