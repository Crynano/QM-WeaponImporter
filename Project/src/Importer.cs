using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates.Descriptors;
using QM_WeaponImporter.Templates;

namespace QM_WeaponImporter
{
    public static class Importer
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

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
            CreateExampleFile(new BulletTemplate(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(new LocalizationTemplate(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(MeleeWeaponTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(RangedWeaponTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(FactionTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(new CustomItemContentDescriptor(), Path.Combine(rootPath, "Examples"));
        }

        private static void CreateExampleFile<T>(T objectType, string folderPath)
        {
            string content = JsonConvert.SerializeObject(objectType, Formatting.Indented);
            Directory.CreateDirectory(folderPath);
            CreateFile(Path.Combine(folderPath, $"example_{objectType.GetType().Name}.json"), content);
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
    }
}