using MGSC;
#if DEBUG
using QM_WeaponImporter.Services;
#endif

namespace QM_WeaponImporter
{
    public static class Main
    {
        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void Start(IModContext context)
        {
            API.LoadModConfig(ConfigDirectories.WeaponImporterName, context.ModContentPath);
#if DEBUG
            ExamplesManager.CreateExampleFiles(context.ModContentPath);
#endif
        }
    }
}