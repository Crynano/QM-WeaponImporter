namespace QM_WeaponImporter.Templates;
using Newtonsoft.Json;
public abstract class ConfigTableRecordTemplate
{
    [JsonProperty(Order = 1)]
    public string Id { get; set; } = string.Empty;
}
