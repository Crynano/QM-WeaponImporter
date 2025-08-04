using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QM_WeaponImporter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using QM_WeaponImporter.Templates;
using UnityEngine;

namespace QM_WeaponImporter
{
    public static class Importer
    {
        // Code recovered from C68.
        // https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Modded to use ImageConversion on Texture creation.

        internal const string GlobalConfigName = "global_config.json";

        internal static float _imagePixelScaling = 200f;
        public static float ImagePixelScaling
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

        private static SpriteImporter SpriteImporter = new SpriteImporter();

        public static Sprite LoadNewSprite(string path, bool returnDefault = true)
        {
            string finalPath = Path.Combine(DataParser.RootFolder, path);
            return SpriteImporter.Import(finalPath, Vector2.zero, ImagePixelScaling, returnDefault);
        }

        public static Sprite LoadCenteredSprite(string path)
        {
            string finalPath = Path.Combine(DataParser.RootFolder, path);
            return SpriteImporter.Import(finalPath, new Vector2(0.5f, 0f), 100f);
        }

        public static T LoadFileFromBundle<T>(string bundlePath, string fileName) where T : class
        {
            if(string.IsNullOrEmpty(fileName))
            {
                Logger.LogWarning($"fileName was empty or null");
                return null;
            }

            if (string.IsNullOrEmpty(bundlePath))
            {
                Logger.LogWarning($"bundlePath was empty or null");
                return null;
            }

            var completePath = Path.Combine(DataParser.RootFolder, bundlePath);

            if (!File.Exists(completePath)) 
            { 
                Logger.LogWarning($"Could not find bundle with {bundlePath} at {completePath}"); 
                return null; 
            }

            // If file doesnt have the correct extension?
            // if (!Path.HasExtension(completePath)) { Logger.LogError($"Incorrect path at {bundlePath}"); return null; }
            //Logger.LogInfo($"Loading from {bundlePath}");
            // We assume its right
            var loadedBundle = AssetBundle.LoadFromFile(completePath);
            var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
            loadedBundle.Unload(false);
            if (loadedAsset != null)
            {
                //Logger.LogInfo($"Loaded asset correctly! Returning {loadedAsset.GetType()}");
                return loadedAsset;
            }
            else
            {
                Logger.LogWarning($"Asset {fileName} is missing from bundle: {bundlePath}");
                return null;
            }
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
                Logger.LogWarning($"Path was null when importing an audio file");
                return null;
            }
            string finalPath = Path.Combine(DataParser.RootFolder, relativePath);
            if (!File.Exists(finalPath))
            {
                //throw new NullReferenceException($"Audio at {finalPath} does not exist.");
                Logger.LogError($"Sound at {finalPath} does not exist");
                return null;
            }
            //Logger.LogInfo($"Loaded audio successfully from {relativePath}");
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
            string fullPath = Path.Combine(path, GlobalConfigName);
            //Logger.LogInfo($"Loading global config at \"{path}\"");
            if (!File.Exists(fullPath))
            {
                //throw new NullReferenceException($"Path \"{fullPath}\" for GetGlobalConfig is null.");
                Logger.LogError($"Config is null at {fullPath}");
                return null;
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