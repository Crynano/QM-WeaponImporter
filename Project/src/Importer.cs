using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates.Descriptors;
using MGSC;
using Newtonsoft.Json.Serialization;

namespace QM_WeaponImporter
{
    public static class Importer
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

        public const string ItemsFileName = "import_weapons.json";
        public const string FactionsFileName = "import_factionLoot.json";

        internal const string GlobalConfigName = "global_config.json";

        internal static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static Sprite LoadNewSprite(string path, float PixelsPerUnit = 200f)
        {
            string finalPath = Path.Combine(ConfigManager.rootFolder, path);
            if (!File.Exists(finalPath))
            {
                Logger.WriteToLog($"Image does not exist at path: \"{finalPath}\".\nNot loading a any image!");//Loading default sprite.");
                return null;
            }
            Logger.WriteToLog($"Recovering image from: \"{finalPath}\"");
            Texture2D SpriteTexture = LoadTexture(finalPath);
            // Maybe we can adjust the size of the sprite.
            Rect oldRect = new Rect(0, 0, SpriteTexture.width, SpriteTexture.height);
            Sprite NewSprite = Sprite.Create(SpriteTexture, oldRect, new Vector2(0, 0), PixelsPerUnit);
            return NewSprite;
        }

        public static Texture2D LoadTexture(string FilePath)
        {
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                try
                {
                    FileData = File.ReadAllBytes(FilePath);
                    // Size does not matter, it gets overwritten.
                    // TEsting with colour spaces.
                    // Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
                    Tex2D = new Texture2D(2, 2);
                    Tex2D.filterMode = FilterMode.Point;
                    ImageConversion.LoadImage(Tex2D, FileData);
                    return Tex2D;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not load image from {FilePath}. Error: {e.Message}");
                }
            }
            return null;
        }

        /// <summary>
        /// Use this function to create default files so you can have examples on-demand.
        /// </summary>
        /// <param name="rootPath"></param>
        public static void CreateExampleConfigFiles(string rootPath)
        {
            // Items file
            if (!Directory.Exists(rootPath)) 
            {
                throw new NullReferenceException($"Directory {rootPath} does not exist.");
            }
            // Examples
            var weaponListExample = MeleeWeaponTemplate.GetExample();
            string weaponSerialized = JsonConvert.SerializeObject(weaponListExample);
            CreateFile(Path.Combine(rootPath, "example_meleeWeapon.json"), weaponSerialized);

            // Examples
            var rangedWeaponExample = RangedWeaponTemplate.GetExample();
            weaponSerialized = JsonConvert.SerializeObject(rangedWeaponExample);
            CreateFile(Path.Combine(rootPath, "example_rangedWeapon.json"), weaponSerialized);

            // Factions file
            var factionListExample = ExportableFactionList.GetExample();
            weaponSerialized = JsonConvert.SerializeObject(factionListExample);
            CreateFile(Path.Combine(rootPath, "example_factionConfig.json"), weaponSerialized);

            var customItemContentDesc = new CustomItemContentDescriptor();
            string customItemContentDescSerial = JsonConvert.SerializeObject(customItemContentDesc);
            CreateFile(Path.Combine(rootPath, "example_CustomItemContentDescriptor.json"), customItemContentDescSerial);
        }

        public static void CreateGlobalConfig(string rootPath)
        {
            // GlobalConfigName
            // Instance of default ConfigTemplate
            if (!Directory.Exists(rootPath))
            {
                throw new NullReferenceException($"Directory {rootPath} does not exist.");
            }
            
            // Maybe to help with this, import the file if exists, and if defaults are not there, add them?
            CreateFile(Path.Combine(rootPath, GlobalConfigName), JsonConvert.SerializeObject(new ConfigTemplate(), Formatting.Indented));
        }

        public static void CreateGlobalConfig(ConfigTemplate userConfig)
        {
            // GlobalConfigName
            // Instance of default ConfigTemplate
            string rootFolder = userConfig.rootFolder;
            if (!Directory.Exists(rootFolder))
            {
                throw new NullReferenceException($"Directory {rootFolder} does not exist.");
            }

            CreateFile(Path.Combine(rootFolder, GlobalConfigName), JsonConvert.SerializeObject(userConfig, Formatting.Indented));
        }

        // Do not load images with this
        public static T Load<T>(string path)
        {
            string fullPath = Path.Combine(ConfigManager.rootFolder, path);
            if (!File.Exists(fullPath))
            {
                throw new NullReferenceException($"Path \"{fullPath}\" for Item of type {typeof(T).ToString()} is null.");
            }
            string loadedString = File.ReadAllText(fullPath);
            T loadedObject = JsonConvert.DeserializeObject<T>(loadedString);
            return loadedObject;
        }

        public static ConfigTemplate GetGlobalConfig(string path)
        {
            // Well, time to WORKAROUND
            Logger.WriteToLog($"Config Path: {path}");
            string fullPath = Path.Combine(path, GlobalConfigName);
            if (!File.Exists(fullPath))
            {
                throw new NullReferenceException($"Path \"{fullPath}\" for GetGlobalConfig is null.");
            }
            var importedString = File.ReadAllText(fullPath);
            //string[] diskDrive = path.Split(':');
            var config = JsonConvert.DeserializeObject<ConfigTemplate>(importedString);
            //config.rootFolder = diskDrive[0] + ":" + config.rootFolder;
            return config;
        }

        private static void CreateFile(string filePath, string content, bool overrideFile = false)
        {
            if (File.Exists(filePath) && !overrideFile)
            {
                Logger.WriteToLog($"File exists at {filePath}.Not creating a new one.", Logger.LogType.Warning);
                return;
            }
            Logger.WriteToLog($"Creating file {filePath}");
            File.WriteAllText(filePath, content);
        }

        public static T ImportUserWeapons<T>(string fileName)
        {
            string configPath = Path.Combine(AssemblyFolder, fileName);
            if (File.Exists(configPath))
            {
                // Read all and parse it. Does it parse right?
                var json = File.ReadAllText(configPath);
                T userConfig = JsonConvert.DeserializeObject<T>(json);
                return userConfig;
            }
            else
            {
                throw new NullReferenceException($"{fileName} does not exist!");
            }
        }
    }
}