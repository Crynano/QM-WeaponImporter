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
        /// An absolute path to determine the root folder where assets are located.
        /// </summary>
        public string rootFolder { get; set; }

        /// <summary>
        /// In relative pathing. (Relative to RootFolder)
        /// </summary>
        public string descriptorsPath { get; set; }
        public Dictionary<string, string> localizationPaths { get; set; }
        public Dictionary<string, string> folderPaths { get; set; }

        public ConfigTemplate()
        {
            rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/");
            descriptorsPath = "Assets/Descriptors";
            localizationPaths = new Dictionary<string, string> {
                { "item", "Assets/localizations/item_localization.json" }
            };
            folderPaths = new Dictionary<string, string>
            {
                { "ammo", "Assets/Config/ammo" },
                { "meleeweapons", "Assets/Config/meleeweapons" },
                { "rangedweapons", "Assets/Config/rangedweapons" },
                { "itemtransforms", "Assets/Config/itemtransforms" },
                { "itemreceipts", "Assets/Config/itemreceipts" },
                { "workbenchreceipts", "Assets/Config/workbenchreceipts" },
                { "datadisks", "Assets/Config/datadisks" },
                { "factionconfig", "Assets/Config/factionconfig" },
                { "backpacks", "Assets/Config/backpacks" }
            };
        }

        public ConfigTemplate(string rootFolder, string descriptorsPath, Dictionary<string, string> localizationPaths, Dictionary<string, string> folderPaths)
        {
            this.rootFolder = rootFolder;
            this.descriptorsPath = descriptorsPath;
            this.localizationPaths = localizationPaths;
            this.folderPaths = folderPaths;
        }
    }

    // Future updates, maybe sync it with this.
    public enum LoadOrder
    {
        // Descriptors
        ammo,
        rangedweapons,
        meleeweapons,
        // Everything else.
    }
}