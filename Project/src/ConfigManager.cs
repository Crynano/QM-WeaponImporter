using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
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

        private static List<string> ExpectedPaths = new List<string>()
        {
            "meleeweapons",
            "rangedweapons",
            "itemtransforms"
        };

        // Create the global config in the assembly folder.
        // You only send the config over, then everything else is automatic.
        public static bool ImportConfig(ConfigTemplate userConfig)
        {
            try
            {
                Logger.WriteToLog($"Starting import config from: {userConfig.rootFolder}");
                // Start the iteration here.
                // Use the root in local, then load the weapon config, and for each weapon load their resources.
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

                // If everything has succeeded proceed.
                //var configText = File.ReadAllText(Path.Combine(rootFolder, ""));
                //var config = JsonConvert.DeserializeObject<ConfigTemplate>(rootFolder);
                // Here we already have a userConfig. Ignore loading.
                // Get all weapons from userconfig
                Logger.WriteToLog($"Iterating through the folders");
                foreach (var path in userConfig.folderPaths)
                {
                    string folderPath = Path.Combine(rootFolder, path.Value);
                    if (!Directory.Exists(folderPath))
                    {
                        Logger.WriteToLog($"Folder in \"{folderPath}\" does not exist. Ignoring and loading other config files.", Logger.LogType.Warning);
                        continue;
                    }
                    Logger.WriteToLog($"Checking for {path}");
                    DirectoryInfo weaponsDirInfo = new DirectoryInfo(folderPath);
                    FileInfo[] files = weaponsDirInfo.GetFiles("*.json");
                    // Now call the appropiate deserializer.
                    foreach (FileInfo singleFile in files)
                    {
                        // Here we have ALL melee Weapons,
                        // Or ALL Ranged weapons.
                        // or all equipment.
                        // Deserialize, then parse.
                        Logger.WriteToLog($"Iterating through {singleFile.Name}");
                        string configItemContent = File.ReadAllText(Path.Combine(folderPath, singleFile.Name));
                        if (ExpectedPaths.Exists(x => x.ToLower() == path.Key.ToLower()))
                        //if (ExpectedDictionaries.TryGetValue(path.Key, out var typeOfItem))
                        {
                            if (path.Key == "meleeweapons" || path.Key == "rangedweapons")
                            {
                                // Process this one
                                var deserializedItem = TypeToClass(path.Key, configItemContent);
                                // We try? Not fit for error control yet.
                                GameItemCreator.CreateWeapon(deserializedItem);
                            }
                            // TODO: need a way to make this generic to config table entries, switching for each entry type is not ideal
                            // this may not even belong here entirely, seperate reader/parser entirely for these data table entries?
                            else if (path.Key == "itemtransforms")
                            {
                                ItemTransformTemplate deserializedItem = DynamicDeserializer<ItemTransformTemplate>(configItemContent);
                                GameItemCreator.CreateConfigTableEntry(deserializedItem);
                            }
                        }
                        else
                        {
                            // Ignore
                            Logger.WriteToLog($"No parser exists for [{path.Key}]", Logger.LogType.Warning);
                        }
                        Logger.WriteToLog("Analyzing file ended");
                    }
                }
                Logger.WriteToLog($"Configuration success for {userConfig.rootFolder}");
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLog($"Configuration loading for {userConfig.rootFolder} failed.\n{e.Message}\n{e.Source}", Logger.LogType.Error);
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