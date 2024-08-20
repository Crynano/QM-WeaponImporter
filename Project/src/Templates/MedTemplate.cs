using System.Collections.Generic;

namespace QM_WeaponImporter.Templates
{
    public class MedTemplate : MGSC.MedkitRecord
    {
        public static MedTemplate GetExample()
        {
            MedTemplate example = new MedTemplate()
            {
               StatusEffectApplyChance = new Dictionary<string, float>
               {
                   { "ExampleStatusEffectApplyChance", 1.0f }
               },
               FixWeights = new Dictionary<string, float>
               {
                   { "ExampleFixWeights", 1.0f }
               },
               StatusEffectProgression = new Dictionary<string, int> 
               {
                   { "ExampleStatusEffectProgression", 1 }
               },
            };
            return example;
        }
    }
}