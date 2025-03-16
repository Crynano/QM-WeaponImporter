using MGSC;
using Newtonsoft.Json;

namespace QM_WeaponImporter.Templates;

public class TrashRecordTemplate : ItemRecordTemplate
{
    [JsonProperty(Order = 9)]
    public short MaxStack { get; set; } = 1;

    [JsonProperty(Order = 10)]
    public TrashSubtype SubType { get; set; } = TrashSubtype.Resource;

    public static TrashRecordTemplate GetExample()
    {
        return new TrashRecordTemplate() { id = "example_trash", itemClass = ItemClass.Parts };
    }
}