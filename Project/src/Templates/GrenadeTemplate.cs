using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGSC;

namespace QM_WeaponImporter.Templates;
internal class GrenadeTemplate : ItemRecordTemplate, IExplosionParametersTemplate, ExplosionVisualParameters
{
    public int turnsToExplode { get; set; }
    public bool ricochet { get; set; }
    public int maxStack { get; set; }
    public bool visualExplosion { get; set; }
    public int radius { get; set; }
    public bool gainDmgToCreature { get; set; }
    public bool gainDmgToLocation { get; set; }
    public int damage { get; set; }
    public string damageType { get; set; }
    public float woundChance { get; set; }
    public bool throwback { get; set; }
    public float throwbackChance { get; set; }
    public bool throwbackDependOnRadius { get; set; }
    public bool stun { get; set; }
    public float stunChance { get; set; }
    public int stunDuration { get; set; }
    public bool stunDependOnRadius { get; set; }
    public bool propagateFire { get; set; }
    public float propagateFireChance { get; set; }
    public float largeFireChance { get; set; }
    public bool fireDependOnRadius { get; set; }
    public bool isPlayerFire { get; set; }
    public bool ignoreMines { get; set; }
    public bool gainDmgToMonolith { get; set; }
    public bool propagateLiquid { get; set; }
    public string liquidType { get; set; }
    public float visualDelay { get; set; }
    public bool shakeOnExplosion { get; set; }
    public int clearGibsRadiusInPixels { get; set; }

    public GrenadeTemplate()
    {
        inventorySortOrder = 9;
    }
}
