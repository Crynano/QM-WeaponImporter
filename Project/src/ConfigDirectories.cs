using System.IO;
using System.Reflection;
using UnityEngine;

namespace QM_WeaponImporter
{
    // Thanks NBK_RedSpy!
    public static class ConfigDirectories
    {
        public static string WeaponImporterName { get; private set; } = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// The Quasimorph_Mods folder that is parallel to the game's folder.
        /// This is a workaround for Quasimorph syncing and overwriting all files in the 
        /// Game's App Data folder.
        /// </summary>
        public static string AllModsConfigFolder { get; set; } = Path.Combine(Application.persistentDataPath, "../Quasimorph_ModConfigs/");

        public static void CreateLogFolders(string modName)
        {
            var folderPath = Path.Combine(AllModsConfigFolder, modName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
