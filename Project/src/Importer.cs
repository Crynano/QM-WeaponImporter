using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QM_WeaponImporter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace QM_WeaponImporter
{
    public static class Importer
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

        internal static float _imagePixelScaling = 200f;
        public static float imagePixelScaling
        {
            get { return _imagePixelScaling; }
            set
            {
                if (value < 100f)
                {
                    Logger.LogWarning($"imagePixelScaling is being set below 100. Issues may occur.\nValue: {value}");
                }
                _imagePixelScaling = value;
            }
        }

        internal const string GlobalConfigName = "global_config.json";

        internal static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static string DefaultConfigPath => Path.Combine(AssemblyFolder, GlobalConfigName);
        public static Sprite LoadNewSprite(string path)
        {
            string finalPath = Path.Combine(ConfigManager.rootFolder, path);
            if (!File.Exists(finalPath))
            {
                Logger.LogInfo($"Image does not exist at path: \"{path}\"\nFull path: \"{finalPath}\"");
                // Return a white texture why not.
                return Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 2, 2), Vector3.zero);
            }
            Logger.LogInfo($"Recovering image at: \"{path}\"\nFull path: \"{finalPath}\"");
            Texture2D SpriteTexture = LoadTexture(finalPath);
            // Maybe we can adjust the size of the sprite.
            Rect oldRect = new Rect(0, 0, SpriteTexture.width, SpriteTexture.height);
            Sprite NewSprite = Sprite.Create(SpriteTexture, oldRect, new Vector2(0, 0), imagePixelScaling);
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
                    // Linear must be set to true.
                    Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
                    Tex2D.filterMode = FilterMode.Point;
                    ImageConversion.LoadImage(Tex2D, FileData);
                    return Tex2D;
                }
                catch (Exception e)
                {
                    Logger.LogError($"Could not load image from {FilePath}. Error: {e.Message}");
                }
            }
            return null;
        }

        private static IAudioImporter<AudioClip> audioImporter = new Services.UnityFileAudioImporter();

        public static AudioClip[] ImportAudio(List<string> audioPaths)
        {
            List<AudioClip> result = new List<AudioClip>();
            foreach (string audioPath in audioPaths)
            {
                var audioRes = ImportAudio(audioPath);
                if (audioRes != null)
                {
                    result.Add(audioRes);
                }
            }
            return result.ToArray();
        }

        public static AudioClip ImportAudio(string relativePath)
        {
            // Use the new UnityAudioModule
            // Give a path, then import. If file does not exist then not
            if (string.IsNullOrEmpty(relativePath))
            {
                Logger.LogError($"Relative path when importing audio was null!");
                return null;
            }
            string finalPath = Path.Combine(ConfigManager.rootFolder, relativePath);
            if (!File.Exists(finalPath))
            {
                //throw new NullReferenceException($"Audio at {finalPath} does not exist.");
                Logger.LogError($"Sound at {finalPath} does not exist");
                return null;
            }

            return audioImporter.Import(finalPath);
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
            FilesManager.CreateFile(Path.Combine(rootPath, GlobalConfigName), JsonConvert.SerializeObject(new ConfigTemplate(), Formatting.Indented));
        }

        //public static void CreateGlobalConfig(ConfigTemplate userConfig)
        //{
        //    // GlobalConfigName
        //    // Instance of default ConfigTemplate
        //    string rootFolder = userConfig.rootFolder;
        //    if (!Directory.Exists(rootFolder))
        //    {
        //        throw new NullReferenceException($"Directory {rootFolder} does not exist.");
        //    }

        //    FilesManager.CreateFile(Path.Combine(rootFolder, GlobalConfigName), JsonConvert.SerializeObject(userConfig, Formatting.Indented));
        //}

        public static ConfigTemplate GetGlobalConfig(string path)
        {
            Logger.LogInfo($"Loading global config at \"{path}\"");
            string fullPath = Path.Combine(path, GlobalConfigName);
            if (!File.Exists(fullPath))
            {
                throw new NullReferenceException($"Path \"{fullPath}\" for GetGlobalConfig is null.");
            }
            var importedString = File.ReadAllText(fullPath);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var resolver = new DefaultContractResolver();
            resolver.DefaultMembersSearchFlags = resolver.DefaultMembersSearchFlags;
            settings.ContractResolver = resolver;
            var config = JsonConvert.DeserializeObject<ConfigTemplate>(importedString, settings);

            return config;
        }
    }
}