using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QM_WeaponImporter
{
    [Serializable]
    public class ConfigTemplate
    {
        /// <summary>
        /// In relative pathing. (Relative to RootFolder)
        /// </summary>
        public string descriptorsPath { get; set; }

        /// <summary>
        /// Path to the localization file / folder
        /// </summary>
        public Dictionary<string, string> localizationPaths { get; set; }

        /// <summary>
        /// Sets the pixels per unit for loading images. 50% is double the size. 200% is half the size.
        /// </summary>
        public float imagePixelScale { get; set; }

        /// <summary>
        /// Folder paths for all config files
        /// Could include sounds, localization, and more
        /// </summary>
        public Dictionary<string, string> folderPaths { get; set; }

        public ConfigTemplate()
        {

        }

        public static ConfigTemplate GetDefault()
        {
            //rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/");
            return new ConfigTemplate()
            {
                imagePixelScale = 200f,
                descriptorsPath = "Assets/Descriptors",
                localizationPaths = new Dictionary<string, string>()
                {
                    { "item", "Assets/Localization/Items"},
                    { "Ui", "Assets/Localization/UI" },
                    { "Firemode", "Assets/Localization/Firemodes" },
                    { "faction", "Assets/Localization/Factions" },
                    { "station", "Assets/Localization/Stations" }
                },
                folderPaths = new Dictionary<string, string>
                {
                    { "trash", "Assets/Config/trash" },
                    { "ammo", "Assets/Config/ammo" },
                    { "firemodes", "Assets/Config/firemodes" },
                    { "meleeweapons", "Assets/Config/meleeweapons" },
                    { "rangedweapons", "Assets/Config/rangedweapons" },
                    { "backpacks", "Assets/Config/backpacks" },
                    { "vests", "Assets/Config/vests" },
                    { "helmets", "Assets/Config/helmets" },
                    { "armors", "Assets/Config/armors" },
                    { "leggings", "Assets/Config/leggings" },
                    { "boots", "Assets/Config/boots" },
                    { "repairs", "Assets/Config/repairs" },
                    { "grenades", "Assets/Config/grenades" },
                    { "mines", "Assets/Config/mines" },
                    { "medkits", "Assets/Config/medkits" },
                    { "consumables", "Assets/Config/consumables" },
                    { "itemtransforms", "Assets/Config/itemtransforms" },
                    { "itemreceipts", "Assets/Config/itemreceipts" },
                    { "workbenchreceipts", "Assets/Config/workbenchreceipts" },
                    { "datadisks", "Assets/Config/datadisks" },
                    { "factionconfig", "Assets/Config/factionconfig" },
                }
            };
        }

        public ConfigTemplate(string descriptorsPath, float imagePixelScale, Dictionary<string, string> localizationPaths, Dictionary<string, string> folderPaths)
        {
            this.descriptorsPath = descriptorsPath;
            this.imagePixelScale = imagePixelScale;
            this.localizationPaths = localizationPaths;
            this.folderPaths = folderPaths;
        }
    }

    // Future updates, maybe sync it with this.
    public enum LoadOrder
    {
        // Descriptors
        sounds,
        trash,
        descriptors,
        itemreceipts,
        itemtransforms,
        ammo,
        rangedweapons,
        meleeweapons,
        // Everything else.
    }
}