using Newtonsoft.Json;
using QM_WeaponImporter.Templates;

namespace QM_WeaponImporter;

public class UsableItemRecordTemplate : ItemRecordTemplate
{
    [JsonProperty(Order = 9)]
    public int MaxUsage { get; set; }

    [JsonProperty(Order = 10)]
    public int UsageCost { get; set; }

    [JsonIgnore]
    public bool IsUsable => UsageCost < MaxUsage;
}
