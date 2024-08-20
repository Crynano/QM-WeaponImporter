using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates.Descriptors;
using QM_WeaponImporter.Templates;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

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
                    Logger.WriteToLog($"imagePixelScaling is being set below 100. Issues may occur.\nValue: {value}", Logger.LogType.Warning);
                }
                _imagePixelScaling = value;
            }
        }

        internal const string GlobalConfigName = "global_config.json";

        internal static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static Sprite LoadNewSprite(string path)
        {
            string finalPath = Path.Combine(ConfigManager.rootFolder, path);
            if (!File.Exists(finalPath))
            {
                Logger.WriteToLog($"Image does not exist at path: \"{path}\"\nFull path: \"{finalPath}\"");
                return null;
            }
            Logger.WriteToLog($"Recovering image at: \"{path}\"\nFull path: \"{finalPath}\"");
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
                    Logger.WriteToLog($"Could not load image from {FilePath}. Error: {e.Message}", Logger.LogType.Error);
                }
            }
            return null;
        }

        public static AudioClip[] ImportAudio(List<string> audioPaths)
        {
            return new AudioClip[0];
            List<AudioClip> result = new List<AudioClip>();
            foreach (string audioPath in audioPaths)
            {
                result.Add(importAudio(audioPath));
            }
            return result.ToArray();
        }

        public static AudioClip ImportAudio(string relativePath)
        {
            return importAudio(relativePath);
        }

        private static AudioClip importAudio(string relativePath)
        {
            throw new NotImplementedException($"Audio is currently not implemented.");
            return null;
            // Give a path, then import. If file does not exist then not
            string finalPath = Path.Combine(ConfigManager.rootFolder, relativePath);
            if (!File.Exists(finalPath))
            {
                //throw new NullReferenceException($"Audio at {finalPath} does not exist.");
                Logger.WriteToLog($"Sound at {finalPath} does not exist", Logger.LogType.Error);
                return null;
            }

            //Get file
            Logger.WriteToLog($"Trying to recover audio: {relativePath}");
            int sampleRate = 44100;
            int channels = 2;
            int position = 0;
            int frequency = 440;
            var samples = new float[sampleRate * channels];
            int itemIndex = 0;
            var audioBytes = File.ReadAllBytes(finalPath);
            for(int i = 0; i < samples.Length; i = i+2)
            {
                samples[i] = audioBytes[itemIndex];
                samples[i + 1] = audioBytes[itemIndex];
                itemIndex++;
            }

            var matchPieceAudio = AudioClip.Create("SampleBeep", sampleRate, channels, frequency, false);
            matchPieceAudio.SetData(samples, 0);

            matchPieceAudio.name = relativePath;
            //matchPieceAudio.LoadAudioData();
            Logger.WriteToLog(matchPieceAudio == null ? "Success " : "Failed " + $"when recovering audio {relativePath}");
            return matchPieceAudio;
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
            CreateExampleFile(MedTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(new MGSC.FoodRecord(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(new MGSC.TrashRecord(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(VestTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(HelmetTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(ArmorTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(LeggingsTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(BootsTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(RepairTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
            CreateExampleFile(GrenadeTemplate.GetExample(), Path.Combine(rootPath, "Examples"));
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
            Logger.WriteToLog($"Loading global config at \"{path}\"");
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

        private static void CreateExampleFile<T>(T objectType, string folderPath)
        {
            string content = JsonConvert.SerializeObject(objectType, Formatting.Indented);
            Directory.CreateDirectory(folderPath);
            CreateFile(Path.Combine(folderPath, $"example_{objectType.GetType().Name}.json"), content);
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