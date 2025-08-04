using System;
using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
using System.IO;
using MGSC;

namespace QM_WeaponImporter.Services
{
    internal static class ExamplesManager
    {
        private static string _rootPath = string.Empty;

        public static void CreateExampleFiles(string rootPath)
        {
            _rootPath = Path.Combine(rootPath, "Examples");

            CreateExampleFile(new BulletTemplate());
            CreateExampleFile(new AmmoRecordTemplate());
            CreateExampleFile(LocalizationTemplate.GetExample());
            CreateExampleFile(MeleeWeaponTemplate.GetExample());
            CreateExampleFile(RangedWeaponTemplate.GetExample());
            //CreateExampleFile(FactionTemplate.GetExample(), );
            CreateExampleFile(MedTemplate.GetExample());
            CreateExampleFile(new ConsumableRecord());
            CreateExampleFile(VestTemplate.GetExample());
            CreateExampleFile(HelmetTemplate.GetExample());
            CreateExampleFile(ArmorTemplate.GetExample());
            CreateExampleFile(LeggingsTemplate.GetExample());
            CreateExampleFile(BootsTemplate.GetExample());
            CreateExampleFile(RepairTemplate.GetExample());
            CreateExampleFile(GrenadeTemplate.GetExample());
            CreateExampleFile(TrashRecordTemplate.GetExample());
            CreateExampleFile(new MGSC.ItemTransformationRecord()
            {
                Id = "Example ID",
                OutputItems =
                    [new MGSC.ItemQuantity() { Count = 1, ItemId = "Item ID" }]
            });
            CreateExampleFile(new MGSC.DatadiskRecord());
            CreateExampleFile(FireModeRecordTemplate.GetExample());
            CreateExampleFile(new AugmentationRecordTemplate());
            //CreateExampleFile(new WoundSlotRecord());
            CreateExampleFile(new ImplantRecord());
            CreateExampleFile(ConfigTemplate.GetDefault(), "global_config.json");

            // Custom path for descs
            _rootPath = Path.Combine(_rootPath, "Descriptors");
            
            CreateExampleFile(new CustomWeaponDescriptor());
            CreateExampleFile(new CustomItemContentDescriptor());
            CreateExampleFile(new CustomImplantDescriptor());
            CreateExampleFile(new CustomFireModeDescriptor());
            CreateExampleFile(new CustomWoundSlotDescriptor());
            CreateExampleFile(new CustomAmmoDescriptor());
        }

        private static void CreateExampleFile<T>(T objectType, string overrideName = "")
        {
            if (objectType == null) return;

            if (string.IsNullOrEmpty(_rootPath)) return;

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            };
            
            // Content
            string content = string.Empty;
            try
            {
                content = JsonConvert.SerializeObject(objectType, settings);
            }
            catch (Exception ex)
            {
                // ignore
                return;
            }
           
            // Folder
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
            
            // Naming
            string finalName = !string.IsNullOrEmpty(overrideName)
                ? overrideName
                : $"example_{objectType.GetType().Name}.json";
            
            // Creation
            FilesManager.CreateFile(Path.Combine(_rootPath, finalName), content);
        }
    }
}