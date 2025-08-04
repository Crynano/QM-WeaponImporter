using System;
using System.Collections.Generic;

namespace QM_WeaponImporter.Templates
{
    [Serializable]
    public class ConfigTemplate
    {
        /// <summary>
        /// In relative pathing. (Relative to RootFolder)
        /// </summary>
        public string descriptorsPath { get; set; }

        /// <summary>
        /// Dictionary of descriptors in relative pathing.
        /// Linked to their own custom descriptor type.
        /// </summary>
        public Dictionary<string, string> descriptorsPaths { get; set; }

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
                descriptorsPaths = new Dictionary<string, string>()
                {
                    { "weaponsDescriptors", "Assets/Descriptors/weapons" },
                    { "ammoDescriptors", "Assets/Descriptors/ammo" },
                    { "firemodesDescriptors", "Assets/Descriptors/firemodes" },
                    { "consumableDescriptors", "Assets/Descriptors/consumables" },
                    { "trashDescriptors", "Assets/Descriptors/trash" },
                    { "helmetsDescriptors", "Assets/Descriptors/helmets" },
                    { "armorsDescriptors", "Assets/Descriptors/armors" },
                    { "leggingsDescriptors", "Assets/Descriptors/leggings" },
                    { "bootsDescriptors", "Assets/Descriptors/boots" },
                    { "grenadesDescriptors", "Assets/Descriptors/grenades" },
                    { "implantsDescriptors", "Assets/Descriptors/implants" },
                    { "augmentationsDescriptors", "Assets/Descriptors/augmentations" }, 
                    { "woundSlotsDescriptors", "Assets/Descriptors/woundSlots" },
                },
                localizationPaths = new Dictionary<string, string>()
                {
                    { "item", "Assets/Localization/Items"},
                    { "Ui", "Assets/Localization/UI" },
                    { "Firemode", "Assets/Localization/Firemodes" },
                    { "faction", "Assets/Localization/Factions" },
                    { "station", "Assets/Localization/Stations" },
                    { "alliance", "Assets/Localization/Alliances" },
                    { "woundslot", "Assets/Localization/WoundSlots" },
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
                    { "implants", "Assets/Config/implants" },
                    { "augmentations", "Assets/Config/augmentations" },
                    { "woundSlots", "Assets/Config/woundSlots" },
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