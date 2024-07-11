using MGSC;
using System;

namespace QM_WeaponImporter
{
    public static class Main
    {
        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void Start(IModContext context)
        {
            //Importer.CreateDefaultConfigFiles();
            //Logger.FlushAdditive();
            Importer.CreateExampleConfigFiles(Importer.AssemblyFolder);
            Importer.CreateGlobalConfig(Importer.AssemblyFolder);
            ConfigManager.LoadDefaultParsers();
            try
            {
                ConfigManager.ImportConfig(Importer.GetGlobalConfig(Importer.AssemblyFolder));
            }
            catch (Exception e) 
            {
                Logger.WriteToLog($"Error while importing. \n{e.Message}", Logger.LogType.Error);
            }
            Logger.Flush();
        }

        //private static void ImportWeapons()
        //{
        //    // Call importer
        //    // For each weapon in the list, call create weapon.
        //    var weaponUserConfig = Importer.ImportUserWeapons<ExportableWeaponList>(Importer.ItemsFileName);
        //    if (weaponUserConfig == null)
        //    {
        //        Logger.WriteToLog($"Error when importing user config. Check if files exist in mod directory.");
        //        return;
        //    }
        //    else
        //    {
        //        foreach (MeleeWeaponTemplate weapon in weaponUserConfig.Melee)
        //        {
        //            if (!GameItemCreator.CreateMeleeWeapon(weapon))
        //            {
        //                Logger.WriteToLog($"Melee: [{weapon.id}] could not be loaded");
        //            }
        //        }

        //        foreach (RangedWeaponTemplate weapon in weaponUserConfig.Ranged)
        //        {
        //            if (!GameItemCreator.CreateRangedWeapon(weapon))
        //            {
        //                Logger.WriteToLog($"Ranged: [{weapon.id}] could not be loaded");
        //            }
        //        }
        //    }

        //    var factionUserConfig = Importer.ImportUserWeapons<ExportableFactionList>(Importer.FactionsFileName);
        //    // Then we proceed with the next one.
        //    foreach (FactionTemplate factionInfo in factionUserConfig.Factions)
        //    {
        //        GameItemCreator.AddItemsToFactions(factionInfo);
        //    }
        //}
    }
}