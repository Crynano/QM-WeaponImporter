using System;

namespace QM_WeaponImporter;
public static class API
{
    // This class is what should be used from mods
    // So they can import whatever.
    // They should only provide the global config.
    // As a file path to the global config or as a class instance itself.

    public static bool LoadModConfig(string modName, string rootPath)
    {
        Setup(modName);
        try
        {
            var result = ConfigManager.ImportConfig(rootPath);
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError($"Mod with path {rootPath} could not be loaded.\n{e.Message}\n{e.Source}");
        }
        finally
        {
            Logger.FlushAdditive();
        }
        return false;
    }

    public static bool LoadModConfig(string modName, ConfigTemplate userConfig, string dataFolderRoot)
    {
        Setup(modName);
        try
        {
            if (userConfig == null)
            {
                Logger.LogWarning($"UserConfig sent was null!");
            }
            else
            {
                var result = ConfigManager.ImportConfig(userConfig, dataFolderRoot);
                return result;
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Configuration with root: {dataFolderRoot} could not be loaded.\n{e.Message}\n{e.Source}");
        }
        finally
        {
            Logger.FlushAdditive();
        }
        return false;
    }

    private static void Setup(string modName)
    {
        ConfigDirectories.CreateLogFolders(modName);
        Logger.SetConfig(modName);
    }
}
