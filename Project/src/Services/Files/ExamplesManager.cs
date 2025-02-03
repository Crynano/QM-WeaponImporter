using Newtonsoft.Json;
using QM_WeaponImporter.Templates;
using System.IO;

namespace QM_WeaponImporter.Services
{
    internal class ExamplesManager
    {
        public void CreateExampleFiles(string rootPath)
        {
            CreateExampleFile(new BulletTemplate(), rootPath);
            CreateExampleFile(LocalizationTemplate.GetExample(), rootPath);
            CreateExampleFile(MeleeWeaponTemplate.GetExample(), rootPath);
            CreateExampleFile(RangedWeaponTemplate.GetExample(), rootPath);
            //CreateExampleFile(FactionTemplate.GetExample(), rootPath);
            CreateExampleFile(new CustomItemContentDescriptor(), rootPath);
            CreateExampleFile(MedTemplate.GetExample(), rootPath);
            CreateExampleFile(new MGSC.ConsumableRecord(), rootPath);
            CreateExampleFile(new MGSC.TrashRecord(), rootPath);
            CreateExampleFile(VestTemplate.GetExample(), rootPath);
            CreateExampleFile(HelmetTemplate.GetExample(), rootPath);
            CreateExampleFile(ArmorTemplate.GetExample(), rootPath);
            CreateExampleFile(LeggingsTemplate.GetExample(), rootPath);
            CreateExampleFile(BootsTemplate.GetExample(), rootPath);
            CreateExampleFile(RepairTemplate.GetExample(), rootPath);
            CreateExampleFile(GrenadeTemplate.GetExample(), rootPath);
            CreateExampleFile(new MGSC.ItemTransformationRecord()
            {
                Id = "Example ID",
                OutputItems =
                [new MGSC.ItemQuantity() { Count = 1, ItemId = "Item ID" }]
            }, rootPath);
            CreateExampleFile(new MGSC.DatadiskRecord(), rootPath);
            CreateExampleFile(FireModeRecordTemplate.GetExample(), rootPath);
        }

        private void CreateExampleFile<T>(T objectType, string root)
        {
            if (objectType == null) return;

            if (string.IsNullOrEmpty(root)) return;

            string content = JsonConvert.SerializeObject(objectType, Formatting.Indented);
            string folderPath = Path.Combine(root, "Examples");
            Directory.CreateDirectory(folderPath);
            FilesManager.CreateFile(Path.Combine(folderPath, $"example_{objectType.GetType().Name}.json"), content);
        }
    }
}