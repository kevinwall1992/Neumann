using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Appliance : Profession
{
    public float EnergyPerSecond;
    public float ToolsPerSecond;
}

public abstract class ApplianceBehavior : EnergyRequestBehavior
{
    public override float EnergyPerSecond { get { return Appliance.EnergyPerSecond; } }

    public virtual bool IsProducingTools { get { return false; } }

    public override Pile BaseUsagePerSecond
    {
        get
        {
            return base.BaseUsagePerSecond.PutIn(
                Resource.Tools,
                Mathf.Max(0, IsProducingTools ? 0 : Appliance.ToolsPerSecond));
        }
    }

    public override List<Variable> Variables
    {
        get
        {
            return base.Variables.Merged(Utility.List(
                new FunctionVariable(Scene.Main.Style.VariableNames.Productivity, 
                                     () => UsageFraction)));
        }
    }

    public Appliance Appliance { get { return GetComponent<Appliance>(); } }
}
