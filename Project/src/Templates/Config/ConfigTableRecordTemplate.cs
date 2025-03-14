namespace QM_WeaponImporter.Templates;
using Newtonsoft.Json;
public abstract class ConfigTableRecordTemplate
{
    [JsonProperty(Order = 1)]
    public string id { get; set; } = string.Empty;
}
