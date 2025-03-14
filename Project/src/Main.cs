using MGSC;
using QM_WeaponImporter.Services;

namespace QM_WeaponImporter
{
    public static class Main
    {
        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void Start(IModContext context)
        {
            API.LoadModConfig(ConfigDirectories.WeaponImporterName, ConfigDirectories.WeaponImporterRootPath);
#if DEBUG
            ExamplesManager.CreateExampleFiles(ConfigDirectories.WeaponImporterRootPath);
#endif
        }
    }
}