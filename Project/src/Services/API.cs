using System;

namespace QM_WeaponImporter;
public class API
{
    // This class is what should be used from mods
    // So they can import whatever.
    // They should only provide the global config.
    // As a file path to the global config or as a class instance itself.

    // This should be the thing, but if a user wants to send over a class, go ahead.
    public bool LoadModConfig(string rootPath)
    {
        try
        {
            var result = ConfigManager.ImportConfig(rootPath);
            Logger.FlushAdditive();
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

    public bool LoadModConfig(ConfigTemplate userConfig, string dataFolderRoot)
    {
        try
        {
            if (userConfig == null)
            {
                Logger.LogWarning($"UserConfig sent was null!");
            }
            else
            {
                var result = ConfigManager.ImportConfig(userConfig, dataFolderRoot);
                Logger.FlushAdditive();
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
}
