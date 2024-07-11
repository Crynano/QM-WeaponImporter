using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QM_WeaponImporter
{
    [Serializable]
    public class ConfigTemplate
    {
        public string rootFolder { get; set; }
        public Dictionary<string, string> folderPaths { get; set; }

        public ConfigTemplate()
        {
            rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/");
            folderPaths = new Dictionary<string, string>
            {
                { "meleeweapons", "Assets/Config/meleeweapons" },
                { "rangedweapons", "Assets/Config/rangedweapons" },
                { "itemtransforms", "Assets/Config/itemtransforms" },
            };
        }

        public ConfigTemplate(string rootFolder, Dictionary<string, string> folderPaths)
        {
            this.rootFolder = rootFolder;
            this.folderPaths = folderPaths;
        }
    }

    // Future updates, maybe sync it with this.
    public enum GameItemType
    {
        meleeweapons,
        rangedweapons
    }
}