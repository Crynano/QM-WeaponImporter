using System;
using MGSC;
using QM_WeaponImporter.Services.Items;
using QM_WeaponImporter.Templates;
using UnityEngine;

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
            var result = DataParser.ImportConfig(rootPath);
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

    /// <summary>
    /// Automatically gets the reference from the given Mod context provided by MGSC
    /// </summary>
    /// <param name="modName">The name of the mod. No restrictions.</param>
    /// <param name="context">Mod context provided by MGSC. It is used to define the path.</param>
    /// <returns></returns>
    public static bool LoadModConfig(string modName, MGSC.IModContext context)
    {
        Setup(modName);
        try
        {
            var result = DataParser.ImportConfig(context.ModContentPath);
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError($"Mod with path {context.ModContentPath} could not be loaded.\n{e.Message}\n{e.Source}");
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
                var result = DataParser.ImportConfig(userConfig, dataFolderRoot);
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
        ConfigManager.CreateLogFolders(modName);
        Logger.SetConfig(modName);
    }

    #region Helper Methods
    
    /// <summary>
    /// Gets a property from an existing in-game item from the MGSC.Data.Items list.
    /// </summary>
    /// <param name="id">The id of the item.</param>
    /// <param name="propertyName">Property Name you want to obtain. CASE-SENSITIVE</param>
    /// <typeparam name="T">Descriptor type. e.g. ItemDescriptor, WoundDescriptor, etc</typeparam>
    /// <returns></returns>
    public static object GetPropertyFromItem<T>(string id, string propertyName) where T : ScriptableObject
    {
        return ItemPropertyObtainer.GetPropertyFromItem<T>(id, propertyName);
    }

    /// <summary>
    /// Gets a property from an existing item contained in a given list.
    /// </summary>
    /// <param name="id">The id of the item.</param>
    /// <param name="propertyName">Property Name you want to obtain. CASE-SENSITIVE</param>
    /// <param name="list">The list where the item is contained. Usually MGSC.Data.Items or similar.</param>
    /// <typeparam name="T">Record type. For example WoundRecord. Must match the Record type from parameter T2.
    /// e.g. T:WoundRecord, T2:WoundDescriptor.</typeparam>
    /// <typeparam name="T2">Descriptor type. For example WoundDescriptor. Must match the Record type from parameter T.
    /// e.g. T:WoundRecord, T2:WoundDescriptor.</typeparam>
    /// <returns></returns>
    public static object GetPropertyFromList<T, T2>(string id, string propertyName, ConfigRecordCollection<T> list)
        where T : ConfigTableRecord where T2 : ScriptableObject
    {
        return ItemPropertyObtainer.GetPropertyFromList<T, T2>(id, propertyName, list);
    }
    
    /// <summary>
    /// Adds localized text to a specific language.
    /// </summary>
    /// <param name="fullyQualifiedKey">The full key, containing category and identifier.
    /// e.g. item.example_id.name, perk.item_example.desc</param>
    /// <param name="text">The localized text to be displayed.</param>
    /// <param name="language">Language enum from MGSC.Localization.Lang</param>
    public static void AddLocalization(string fullyQualifiedKey, string text, MGSC.Localization.Lang language)
    {
        LocalizationHelper.AddLocalization(fullyQualifiedKey, text, language);
    }

    /// <summary>
    /// Adds the same text to all languages.
    /// </summary>
    /// <param name="fullyQualifiedKey">The full key, containing category and identifier.
    /// e.g. item.example_id.name, perk.item_example.desc</param>
    /// <param name="text">The localized text to be displayed.</param>
    public static void AddLocalizationToAllDictionaries(string fullyQualifiedKey, string text)
    {
        LocalizationHelper.AddLocToAllDictionaries(fullyQualifiedKey, text);
    }

    #endregion
}
