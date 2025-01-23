﻿using MGSC;
using System;

namespace QM_WeaponImporter
{
    public static class Main
    {
        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void Start(IModContext context)
        {
            Logger.LogInfo("Starting ModConfigLoad from QM_WeaponImporter");
            try
            {
                // This library (for testing) loads from the same folder too.
                if (ConfigManager.ImportDefaultConfig())
                {
                    Logger.LogInfo("Test mod is loaded!");
                }
                // it only use is as API.
                Logger.LogInfo($"QM_LiteItemImporter API Loaded!");
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while importing. \n{e.Message}\n{e.InnerException}\n{e.StackTrace}");
            }
            finally
            {
                Logger.FlushAdditive();
            }
        }
    }
}