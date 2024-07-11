using QM_WeaponImporter.Templates;
using System;
using System.Collections.Generic;

[Serializable]
public class ItemTransformTemplate : ConfigTableRecordTemplate
{
    public List<string> outputItems { get; set; }

    public ItemTransformTemplate()
    {
        outputItems = new List<string>();
    }
}