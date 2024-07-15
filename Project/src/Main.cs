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
                Logger.WriteToLog($"Error while importing. \n{e.Message}\n{e.InnerException}", Logger.LogType.Error);
            }
            Logger.Flush();
        }
    }
}