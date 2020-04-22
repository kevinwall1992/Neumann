using UnityEngine;
using System.Collections;

public abstract class Appliance : Profession
{
    public float EnergyPerSecond;
    public float ToolsPerSecond;
}

public abstract class ApplianceBehavior : EnergyRequestBehavior
{
    public override float EnergyPerSecond { get { return Appliance.EnergyPerSecond; } }

    public override Pile BaseUsagePerSecond
    {
        get { return base.BaseUsagePerSecond.PutIn(Resource.Tools, Appliance.ToolsPerSecond); }
    }

    public Appliance Appliance { get { return GetComponent<Appliance>(); } }
}
