using System.IO;

namespace QM_WeaponImporter.Services
{
    internal static class FilesManager
    {
        public static void CreateFile(string filePath, string content, bool overrideFile = false)
        {
            if (File.Exists(filePath) && !overrideFile)
            {
                Logger.LogWarning($"Not creating file. File already exists at {filePath}.");
                return;
            }
            Logger.LogInfo($"Creating file {filePath}");
            File.WriteAllText(filePath, content);
        }
    }
}