using MGSC;
using System;

namespace QM_WeaponImporter
{
    public static class Main
    {
        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void Start(IModContext context)
        {
            Importer.CreateExampleConfigFiles(Importer.AssemblyFolder);
            Importer.CreateGlobalConfig(Importer.AssemblyFolder);
            ConfigManager.LoadDefaultParsers();
            try
            {
                ConfigManager.ImportConfig(Importer.GetGlobalConfig(Importer.AssemblyFolder));
            }
            catch (Exception e) 
            {
                Logger.WriteToLog($"Error while importing. \n{e.Message}\n{e.InnerException}\n{e.StackTrace}", Logger.LogType.Error);
            }
            Logger.Flush();
        }
    }
}