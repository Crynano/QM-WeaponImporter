using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

namespace QM_WeaponImporter
{
    public static class Importer
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

        internal const string ItemsFileName = "import_weapons.json";

        internal static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static Sprite LoadNewSprite(string fileName, float PixelsPerUnit = 100.0f)
        {
            string fullPath = Path.Combine(AssemblyFolder, @fileName);
            if (!File.Exists(fullPath)) 
            {
                Logger.WriteToLog($"Image does not exist at path: \"{fullPath}\"\nNot loading a any image!");//Loading default sprite.");
                return null;
            }
            Logger.WriteToLog($"Recovering image from: \"{fullPath}\"");
            Texture2D SpriteTexture = LoadTexture(fullPath);
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
                    Tex2D = new Texture2D(2,2);
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

        public static ExportableWeaponList CreateDefaultConfigFile()
        {
            string configPath = Path.Combine(AssemblyFolder, ItemsFileName);
            if (File.Exists(configPath))
            {
                Logger.WriteToLog($"Config file exists at {configPath}.\nNot creating a new default one.");
                return null;
            }
            var defaultConfig = ExportableWeaponList.GetExampleWeapon();
            string weaponExampleString = JsonConvert.SerializeObject(defaultConfig);
            File.WriteAllText(configPath, weaponExampleString);
            return defaultConfig;
        }

        public static ExportableWeaponList ImportUserWeapons()
        {
            string configPath = Path.Combine(AssemblyFolder, ItemsFileName);
            if (File.Exists(configPath))
            {
                // Read all and parse it. Does it parse right?
                var json = File.ReadAllText(configPath);
                var userConfig = JsonConvert.DeserializeObject<ExportableWeaponList>(json);
                return userConfig;
            }
            else
            {
                throw new NullReferenceException($"{ItemsFileName} does not exist!");
            }
        }
    }
}