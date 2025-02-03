using MGSC;
using System;

namespace QM_WeaponImporter.Templates;
[Serializable]
public class ItemTransformationRecordTemplate : ItemTransformationRecord
{
    public new string Id { get; set; } = string.Empty;
}